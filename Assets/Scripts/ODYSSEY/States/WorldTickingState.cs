using UnityEngine;
using System;
using Odyssey.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

namespace Odyssey
{
    public class WorldTickingState : IState
    {
        IMomentumContext _c;

        IWorldData worldData;
        ILODSystem lodSystem;
        Transform avatarTransform;
        ISessionData sessionData;
        ITextureService textureService;
        IInfoUIDriver infoUIDriver;
        HS.IThirdPersonController thirdPersonController;
        IMinimapDriver minimapDriver;
        IFollowUserController followUserController;
        IUserInteraction userInteraction;

        StateController[] _controllers;
        List<WorldObject> lodNearByObjects = new List<WorldObject>();

        public WorldTickingState(IMomentumContext context)
        {
            _c = context;
            _controllers = new StateController[]
            {
                new LookAtWispController(context),
                new WorldController(context),
                new WorldObjectsMetadataController(context),
                new WorldEffectsController(context),
                new NotificationsController(context),
                new UserPositionController(context),
                new TextureUpdatesController(context),
                new ReactEventsController(context),
                new MomentumAPIController(context)
            };
        }
        public void OnEnter()
        {
            // Do not run, if we don't have an AvatarController (this should never happen, because we have a default one shipped)
            if (_c.Get<ISessionData>().WorldAvatarController == null)
            {
                Logging.LogError("[WorldTickingState] No Avatar found, shutting down..");
                _c.Get<IStateMachine>().SwitchState(typeof(ShutdownState));
            }

            // Cache services that are called in Update()
            lodSystem = _c.Get<ILODSystem>();
            textureService = _c.Get<ITextureService>();
            avatarTransform = _c.Get<ISessionData>().WorldAvatarController.transform;
            sessionData = _c.Get<ISessionData>();
            minimapDriver = _c.Get<IMinimapDriver>();
            thirdPersonController = _c.Get<HS.IThirdPersonController>();
            infoUIDriver = _c.Get<IInfoUIDriver>();
            worldData = _c.Get<IWorldData>();
            followUserController = _c.Get<IFollowUserController>();
            userInteraction = _c.Get<IUserInteraction>();

            // InfoUI Click Event
            infoUIDriver.OnLabelClicked_Event += OnInofUILabelClicked;

            // LOD
            lodSystem.LODDistance1 = sessionData.worldDefinition.LOD1Distance;
            lodSystem.LODDistance2 = sessionData.worldDefinition.LOD2Distance;
            lodSystem.LODDistance3 = sessionData.worldDefinition.LOD3Distance;

            lodSystem.StartRunning();


            // Wisp Manager
            _c.Get<IWispManager>().InitWispsPrefabsPool();
            _c.Get<IWispManager>().StartRunning();


            // Start or Enable Various other Services
            _c.Get<IMinimapDriver>().ShowHideMinimap(show: true);
            _c.Get<IResolutionManager>().Enabled = true;

            // PosBus
            _c.Get<IPosBus>().ProcessMessageQueue = true;
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;

            // Capture the keyboard by WebGL, if for some reason, it has been taken away from us
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif

            // OnEnter Controllers
            foreach (var controller in _controllers)
            {
                controller.OnEnter();
            }

            // Hide Loading Screen
            _c.Get<ILoadingScreenManager>().SetLoading(false);

            // Send Events to React and PosBus that the World is spawned and ready
            _c.Get<IReactAPI>().SendReadyToTeleportToReact();
            _c.Get<IReactAPI>().SendLoadingProgress(100);
            _c.Get<IPosBus>().UnityReady();


            // Finalize state
            _c.Get<ISessionData>().WorldIsTicking = true;

            Logging.Log("[WorldTickingState] World is Ticking now!");
        }


        void OnInofUILabelClicked(Guid guid, string label)
        {
            Debug.Log("UI element clicked: " + guid.ToString() + " " + label);

            if (label == "dashboard")
            {
                _c.Get<IReactAPI>().SendClick(guid.ToString(), label);
            }
            else if (label == "flyto")
            {
                _c.Get<ITeleportSystem>().OnTeleportToSpace(guid.ToString());
            }
            else
            {
                _c.Get<IReactAPI>().SendClick(guid.ToString(), label);
            }

        }

        void OnPosBusMessage(IPosBusMessage msg)
        {
            switch (msg)
            {
                case PosBusSignalMsg m:
                    OnPosBusSignal(m.signal);
                    break;
            }
        }

        void OnPosBusSignal(PosBusSignalType signal)
        {
            if (signal == PosBusSignalType.DualConnection)
            {
                Logging.Log("[WorldTickingState] Received Disconnect Signal from PosBus! Exterminating Unity Client!");

                _c.Get<IReactAPI>().ExterminateUnity();

                _c.Get<IStateMachine>().SwitchState(typeof(ShutdownState));
            }
        }


        public void OnExit()
        {
            Debug.Log("World is Not ticking anymore..");

            foreach (var controller in _controllers)
            {
                controller.OnExit();
            }

            _c.Get<ILoadingScreenManager>().SetLoading(true, true);

            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
            infoUIDriver.OnLabelClicked_Event -= OnInofUILabelClicked;

            _c.Get<IMinimapDriver>().ShowHideMinimap(show: false);
            _c.Get<IInfoUIDriver>().Clear();

            _c.Get<IWorldDataService>().ClearAll();
            _c.Get<ITextureCache>().Clear();
            _c.Get<IWorldPrefabHolder>().ClearPreloadList();

            _c.Get<ISessionData>().AvatarCamera = null;
            _c.Get<ISessionData>().WorldIsTicking = false;

            _c.Get<IWispManager>().Clear();
            _c.Get<IWispManager>().Stop();

            _c.Get<ILODSystem>().Stop();

            _c.Get<IMinimapDriver>().Clear();

            _c.Get<IResolutionManager>().Enabled = false;

            _c.Get<IMemoryManager>().CleanGarbage();
        }

        public void Update()
        {
            if (sessionData.AppPaused) return;

            userInteraction.HandleMouseClicks();

            // Run the LOD to get all Nearby objects
            lodSystem.RunLOD(avatarTransform.position);

            // Update Info UI, Disabled for now!
            bool showUIForHovered = !thirdPersonController.IsControlling && !minimapDriver.IsExpanded() && !userInteraction.HasHoveredClickable;
            infoUIDriver.UpdateDriver(Input.mousePosition, showUIForHovered);

            // Run the update loop for the nearby objects
            List<WorldObject> nearby = lodSystem.GetNearby();

            for (var i = 0; i < nearby.Count; ++i)
            {
                AlphaStructureDriver driver = nearby[i].GetStructureDriver();

                if (!driver) continue;

                driver.UpdateBehaviours(Time.deltaTime);
                driver.lastVisit = Time.fixedTime;
            }

            // update textures
            for (var i = 0; i < nearby.Count; ++i)
            {
                // Update the textures for objects that are close to the user
                if (nearby[i].LOD <= 2 && nearby[i].texturesDirty)
                {
                    textureService.UpdateTexturesForObject(nearby[i]);
                }
            }

            // Go through all objects that needs their textures updated, no matter the LOD status
            for (var i = 0; i < worldData.AlwaysUpdateTexturesList.Count; ++i)
            {
                WorldObject wo = worldData.AlwaysUpdateTexturesList[i];

                if (wo.alwaysUpdateTextures && wo.texturesDirty)
                {
                    textureService.UpdateTexturesForObject(wo);
                }
            }

            foreach (var controller in _controllers)
            {
                controller.Update();
            }

            if (followUserController.IsFollowing)
            {
                followUserController.Update(Time.deltaTime);
            }

        }


    }
}