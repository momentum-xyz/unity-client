using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Odyssey.Networking;
using HS;
using System.Linq;

namespace Odyssey
{
    public class InitState : IState
    {
        IMomentumContext _c;
        NetworkingConfigData _networkConfigData;

        public InitState(IMomentumContext context, NetworkingConfigData networkingConfigData)
        {
            _c = context;
            _networkConfigData = networkingConfigData;
        }

        public void OnEnter()
        {
            OnEnterAsync().Forget();
        }

        async UniTask OnEnterAsync()
        {
#if UNITY_EDITOR
            // Get a fresh token, everytime we run the app
            await _networkConfigData.UpdateAccessTokenFromKeycloak();
#endif

            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;

            SessionStats.StartSession();
            SessionStats.AddTime("start");

            // Set the main scene as Active, because
            // if not all the instantiated stuff will be added to Loading and then destroyed
            Scene mainScene = SceneManager.GetSceneByName("MainScene");

            SceneManager.SetActiveScene(mainScene);

            // keep framerate stable
            Application.targetFrameRate = 30;

            // create a sessionID
            Guid sessionID = Guid.NewGuid();
            _c.Get<ISessionData>().SessionID = sessionID.ToString();

            // Git Commit Information
            Debug.Log("Client running from Git " + GitCommit.Description + " version: " + GitCommit.Version);

            // Graphic Card
            Debug.Log("Graphic Card: " + _c.Get<IUnityToReact>().GetGraphicCardFromBrowser());

            // Browser Name
            Debug.Log("Browser: " + _c.Get<IUnityToReact>().GetBrowser());

            // Default Controller Settings
            var controllerSettingsScriptableObj = Resources.LoadAll<ThirdPersonControllerSettings>("").FirstOrDefault();
            if (controllerSettingsScriptableObj != null)
            {
                _c.Get<ISessionData>().ControllerSettings = controllerSettingsScriptableObj.Settings;
            }

            // Setup NetworkingService from configuration
            _c.Get<INetworkingService>().SetupEventHandling();
            _c.Get<INetworkingService>().SetupFromConfig(_networkConfigData);

#if UNITY_WEBGL && !UNITY_EDITOR
            // Call Momentum Loaded event in Start, because
            // at that point in time React has access to the unityInstance
            // if we call it in Awake, unityInstance is not yet available
            _c.Get<IUnityToReact>().SendMomentumLoadedToReact();
#endif
            _c.Get<ILoadingScreenManager>().SetLoading(true, true);

            // Add local mock addressables as they are already downloaded addressables
            if (_c.Get<ISessionData>().NetworkingConfig.useMockAddressables)
            {
                MockAddressablesData mockAddrData = _c.Get<ISessionData>().NetworkingConfig.mockAddressableData;
                for (var i = 0; i < mockAddrData.mockAddressables.Count; ++i)
                {
                    MockAddressable ma = mockAddrData.mockAddressables[i];

                    AddressableAsset ro = new AddressableAsset("", ma.address, "");
                    ro.status = AddressableAssetStatus.Loaded;
                    ro.gameObject = ma.localPrefab;

                    _c.Get<IAddressablesProvider>().AddressablesAssets.Add(ma.ID, ro);

                }
            }

#if UNITY_EDITOR
            // Use a test token if you are in the Editor, because we don't have 
            _c.Get<INetworkingService>().SetUserToken(_c.Get<ISessionData>().NetworkingConfig.AuthenticationToken);
            _c.Get<INetworkingService>().InitNetworkingServices();
            _c.Get<INetworkingService>().ConnectServices();
#endif

        }

        public void Update()
        {

        }

        public void OnExit()
        {

            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {

        }
    }

}