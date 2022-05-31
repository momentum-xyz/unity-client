using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Odyssey;
using Odyssey.Networking;

public class MomentumContextInit : MonoBehaviour
{

    public NetworkingConfigData networkingConfigData;

    IMomentumContext _c;

    private void Awake()
    {
        InitContext();
    }

    private void Start()
    {
        _c.Get<IStateMachine>().IsRunning = true;
        _c.Get<IStateMachine>().SwitchState(typeof(InitState));
    }



    /// <summary>
    /// Creates the Global Context and injects itself into all systems/managers
    /// </summary>
    public void InitContext()
    {

        IMomentumContext context = new MomentumContext();

        context.RegisterService<IAddressablesProvider>(new AddressablesProvider());
        context.RegisterService<ITextureService>(new TextureService());
        context.RegisterService<IWorldData>(new WorldData());
        context.RegisterService<IWorldObjectsStateManager>(new WorldObjectsStateManager());
        context.RegisterService<IStructureMover>(new StructureMover(3.0f));
        context.RegisterService<ITextureCache>(new TextureCache());
        context.RegisterService<IWorldDataService>(new WorldDataService());
        context.RegisterService<IStateMachine>(new StateMachine());
        context.RegisterService<ISessionData>(new SessionData());
        context.RegisterService<ILODSystem>(new OctreeLODSystem());
        context.RegisterService<IFollowUserController>(new FollowUserController());
        context.RegisterService<IUserInteraction>(new UserInteraction());
        context.RegisterService<IReactPosBusClient>(new ReactPosBusClient());

        if (networkingConfigData.useMockData)
        {
            context.RegisterService<IPosBus>(new MockPosBus());
            ((MockPosBus)context.Get<IPosBus>()).Data = networkingConfigData.mockData;
            networkingConfigData.mockData.Init(context);
        }
        else
        {
            var posBus = new PosBus();
            context.RegisterService<IPosBus>(posBus);
        }



        context.RegisterService<IBackendService>(new BackendService());
        context.RegisterService<IRendermanService>(new RendermanService());
        context.RegisterService<IUnityToReact>(new UnityToReact());
        context.RegisterService<IEffectsService>(new EffectsService());

        context.RegisterService<IWorldPrefabHolder>(GetComponentInChildren<IWorldPrefabHolder>(true));
        context.RegisterService<IWispManager>(GetComponentInChildren<IWispManager>(true));
        context.RegisterService<ITeleportSystem>(GetComponentInChildren<ITeleportSystem>(true));
        context.RegisterService<ISpawner>(GetComponentInChildren<ISpawner>(true));
        context.RegisterService<IReactBridge>(GetComponentInChildren<IReactBridge>(true));
        context.RegisterService<IMinimapDriver>(GetComponentInChildren<IMinimapDriver>(true));
        context.RegisterService<IInfoUIDriver>(GetComponentInChildren<IInfoUIDriver>(true));
        context.RegisterService<IResolutionManager>(GetComponentInChildren<IResolutionManager>(true));
        context.RegisterService<ILoadingScreenManager>(GetComponentInChildren<ILoadingScreenManager>(true));
        context.RegisterService<IMemoryManager>(GetComponentInChildren<IMemoryManager>(true));

        // Go through all MonoBehaviours that implements the IRequireContext interface
        IRequiresContext[] contextInjectTargets = GetComponentsInChildren<IRequiresContext>(true);

        for (var i = 0; i < contextInjectTargets.Length; ++i)
        {
            contextInjectTargets[i].Init(context);
        }

        // State Initializiation
        context.Get<IStateMachine>().AddState(new InitState(context, networkingConfigData));
        context.Get<IStateMachine>().AddState(new ReceiveWorldDataState(context));
        context.Get<IStateMachine>().AddState(new SpawnWorldState(context));
        context.Get<IStateMachine>().AddState(new WorldTickingState(context));
        context.Get<IStateMachine>().AddState(new ShutdownState(context));

        _c = context;

#if UNITY_EDITOR
        PosBusLogger.Init(_c.Get<IPosBus>());
#endif

    }

    private void OnDestroy()
    {

#if UNITY_EDITOR
        PosBusLogger.Destroy();
#endif

    }

    private void OnApplicationQuit()
    {
        _c.Get<IStateMachine>().SwitchState(typeof(ShutdownState));
    }

}
