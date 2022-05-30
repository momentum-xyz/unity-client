using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;

public class AppRunner : MonoBehaviour, IRequiresContext
{
    IMomentumContext _c;
    IStateMachine stateMachine;
    INetworkingService networkingService;
    ITextureCache textureCache;
    IPosBus posBus;

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
    }

    void OnDisable()
    {

    }

    public void Dispose()
    {

    }

    void OnNetworkConnectionLost()
    {
        Logging.Log("[AppRunner] App has lost networking connection.. Shutting down..");
        _c.Get<IUnityToReact>().ExterminateUnity();
        _c.Get<ISessionData>().IsUnityTerminatedExternaly = true;
        _c.Get<IStateMachine>().SwitchState(typeof(ShutdownState));
    }

    // Update is called once per frame
    void Update()
    {
        if (_c == null) return;

        networkingService.Update();
        textureCache.Update();
        stateMachine.Update();

    }


}
