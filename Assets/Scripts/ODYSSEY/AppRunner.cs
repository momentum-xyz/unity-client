using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using Cysharp.Threading.Tasks;

public class AppRunner : MonoBehaviour, IRequiresContext
{
    IMomentumContext _c;
    IStateMachine stateMachine;
    INetworkingService networkingService;
    ITextureCache textureCache;
    IPosBus posBus;

    private bool _ignorePositionMessages = false;

    void Awake()
    {
        Application.targetFrameRate = -1;
    }

    public void Init(IMomentumContext context)
    {
        _c = context;

    }

    void OnEnable()
    {
        stateMachine = _c.Get<IStateMachine>();
        networkingService = _c.Get<INetworkingService>();
        textureCache = _c.Get<ITextureCache>();
        posBus = _c.Get<IPosBus>();

        posBus.OnPosBusMessage += OnPosBusMessage;
    }

    void OnDisable()
    {
        posBus.OnPosBusMessage -= OnPosBusMessage;
    }

    public void Dispose()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_c == null) return;

        if (posBus.IsConnected)
        {
            posBus.ProcessReceivedMessagesFromMainThread();
        }


        networkingService.Update();

        textureCache.Update();
        stateMachine.Update();

    }

    void OnPosBusMessage(IPosBusMessage msg)
    {
        switch (msg)
        {
            case PosBusSetWorldMsg m:

                // If we are connected to the same world, do nothing
                // probably means that PosBus disconnected and then re-connected again
                if (_c.Get<ISessionData>().WorldID.Equals(m.worldID.ToString()))
                {
                    // If we re-connected to the same world, while the World was ticking
                    // meaning that posbus disconnected for some reason (restart of service, error)
                    // just send the UnityReady event and nothing else
                    if (posBus.HasReconnected)
                    {
                        posBus.HasReconnected = false;
                        _ignorePositionMessages = true;

                        if (_c.Get<ISessionData>().WorldIsTicking)
                            _c.Get<IPosBus>().UnityReady();
                    }

                    Logging.Log("[NetworkingService] Receive SetWorldMsg for the world we are already connected to: " + m.worldID.ToString());
                    return;
                }

                bool isSwitching = _c.Get<ISessionData>().WorldID.Length == 0 ? false : true;

                Debug.Log(((isSwitching) ? "Switching" : "Connecting") + " to WorldID: " + m.worldID.ToString());

                WorldDefinition worldDefinition = _c.Get<IWorldDataService>().CreateWorldDefinitionFromMsg(m);

                _c.Get<ISessionData>().worldDefinition = worldDefinition;
                _c.Get<ISessionData>().WorldID = m.worldID.ToString();
                _c.Get<IWorldDataService>().GetWorldList().Forget();

                _c.Get<IStateMachine>().SwitchState(typeof(ReceiveWorldDataState));

                break;

            case PosBusSelfPosMsg m:

                // Do not process User position message, after a reconnect 
                if (_ignorePositionMessages)
                {
                    Logging.Log("[NetworkingService] We re-connected from a PosBus Error, so ignoring position message...");
                    _ignorePositionMessages = false;
                    return;
                }

                Debug.Log("SelfPosMsg: " + m.position);

                _c.Get<ISessionData>().GotSelfPositionMsg = true;
                _c.Get<ISessionData>().SelfPosition = m.position;
                break;

            case PosBusSignalMsg m:
                if (m.signal == PosBusSignalType.InvalidToken)
                {
                    Logging.Log("[NetworkingServicer] Got PosBus Signal: Invalid Token");
                    posBus.TokenIsNotValid = true;
                    _c.Get<IReactAPI>().SendInvalidTokenError();
                }
                break;
        }
    }


}
