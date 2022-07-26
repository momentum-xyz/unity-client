using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using Cysharp.Threading.Tasks;

public class AppRunner : MonoBehaviour, IRequiresContext
{
    IMomentumContext _c;
    IStateMachine _stateMachine;
    ITextureCache _textureCache;
    IPosBus _posBus;

    private bool _ignorePositionMessages = false;
    private bool _doPosBusReconnect = false;
    private bool _isFirstConnect = true;  // keeps track is we connected for the first time
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
        _stateMachine = _c.Get<IStateMachine>();
        _textureCache = _c.Get<ITextureCache>();
        _posBus = _c.Get<IPosBus>();

        _posBus.OnPosBusMessage += OnPosBusMessage;
        _c.Get<IUnityJSAPI>().Token_Event += OnReceivedToken;
        _posBus.OnPosBusDisconnected += OnPosBusDisconnected;
        _posBus.OnPosBusConnected += OnPosBusConnected;
    }

    void OnDisable()
    {
        _posBus.OnPosBusMessage -= OnPosBusMessage;
        _c.Get<IUnityJSAPI>().Token_Event -= OnReceivedToken;
        _posBus.OnPosBusDisconnected -= OnPosBusDisconnected;
        _posBus.OnPosBusConnected -= OnPosBusConnected;
    }

    public void Dispose()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_c == null) return;

        if (_posBus.IsConnected)
        {
            _posBus.ProcessReceivedMessagesFromMainThread();
        }

        if (_doPosBusReconnect)
        {
            Logging.Log("[AppRunner] Trying to reconnect to Controller..");
            _doPosBusReconnect = false;
            if (!_posBus.IsConnected) _posBus.Connect();
        }

        _textureCache.Update();
        _stateMachine.Update();

    }

    void OnReceivedToken(string token)
    {
        _c.Get<ISessionData>().ParseToken(token);

        _posBus.UserID = _c.Get<ISessionData>().UserID.ToString();
        _posBus.SessionID = _c.Get<ISessionData>().SessionID;
        _posBus.AuthenticationToken = _c.Get<ISessionData>().Token;
        _posBus.TokenIsNotValid = false;

        if (!_posBus.IsConnected)
        {
            _posBus.Connect();
        }
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
                    if (_posBus.HasReconnected)
                    {
                        _posBus.HasReconnected = false;
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
                    _posBus.TokenIsNotValid = true;
                    _c.Get<IReactAPI>().SendInvalidTokenError();
                }
                break;
        }


    }

    void OnPosBusConnected()
    {
        if (_posBus.IsAuthenticated) return;

        // Authenticate to Controller
        _posBus.SendHandshake(_isFirstConnect ? _c.Get<ISessionData>().NetworkingConfig.domain : "");
        _isFirstConnect = false;

    }

    void OnPosBusDisconnected(PosBusDisconnectError errorCode)
    {
        if (errorCode == PosBusDisconnectError.UNKNOWN)
        {
            // if we still have a valid token, re-connect
            if (!_posBus.TokenIsNotValid)
            {
                // use this flag to trigger a reconnect on the main thread, because the OnDisconnect event, might be received on a different one
                _doPosBusReconnect = true;
            }
            else
            {
                Logging.Log("[AppRunner] Controller has an invalid token, waiting for a valid one to re-connect ");
            }

        }
    }
}
