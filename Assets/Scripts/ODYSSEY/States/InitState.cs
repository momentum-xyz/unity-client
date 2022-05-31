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
            _c.Get<IReactPosBusClient>().OnPosBusConnected += OnReactPosBusConnected;

            OnEnterAsync().Forget();
        }

        async UniTask OnEnterAsync()
        {

#if UNITY_EDITOR
            // Get a fresh token, everytime we run the app
            await _networkConfigData.UpdateAccessTokenFromKeycloak();
            if (_networkConfigData.AuthenticationToken == null) return;
#endif

            SetupNetworkingConfig(_networkConfigData);

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
            // If we are running in the Editor, parse the Token from NetworkConfigData
            // and use that to init and connect PosBus client

            // Get the UserID from the Token
            ParseToken(_c.Get<ISessionData>().NetworkingConfig.AuthenticationToken);

            // Set the Websocket Handler
            _c.Get<IPosBus>().WebsocketHandler = new HybridWS();

            // Init PosBus
            _c.Get<IPosBus>().Init(
                _c.Get<ISessionData>().NetworkingConfig.posBusURL,
                _c.Get<ISessionData>().Token,
                _c.Get<ISessionData>().UserID.ToString(),
                _c.Get<ISessionData>().SessionID,
                _c.Get<ISessionData>().NetworkingConfig.domain
                );

            // Connect
            _c.Get<IPosBus>().Connect();
#endif

#if !UNITY_EDITOR && UNITY_WEBGL
            _c.Get<IReactPosBusClient>().Init();
            _c.Get<IPosBus>().WebsocketHandler = new ReactWS(_c);
            _c.Get<IPosBus>().Init("","","","","");

            _c.Get<IUnityToReact>().SendMomentumLoadedToReact();
#endif
        }

        void SetupNetworkingConfig(NetworkingConfigData data)
        {
            // Clone the original configuration and use the clone 
            // because if we modify something after that
            // that will modify the original data file, because it is a ScripableObject and it is Serialized
            NetworkingConfigData networkingConfigCopy = data.Clone();

#if !UNITY_EDITOR && UNITY_WEBGL
            string domain = DataHelpers.GetDomainFromURL(Application.absoluteURL);
        
            // if we are running on localhost
            // overwrite the domain with one provided in the configuration
            if(domain.Contains("localhost") || domain.Contains("127.0.0.1")) {
                domain = data.localDomainOverwrite;      
            }

            if(!networkingConfigCopy.ignoreApplicationURL)
                networkingConfigCopy.InitFromDomain(domain);
#endif
            networkingConfigCopy.AuthenticationToken = data.AuthenticationToken;

            _c.Get<ISessionData>().NetworkingConfig = networkingConfigCopy;

            _c.Get<IBackendService>().APIEndpoint = _c.Get<ISessionData>().NetworkingConfig.apiEndpoint;
            _c.Get<IRendermanService>().RendermanEndpoint = _c.Get<ISessionData>().NetworkingConfig.rendermanURL;
            _c.Get<IRendermanService>().DefaultHash = _c.Get<ITextureService>().DefaultTextureHash;
        }

        void ParseToken(string token)
        {
            if (token.Length == 0)
            {
                Debug.LogError("Trying to set empty token!");
                return;
            }

            UserTokenContent tokenData = DataHelpers.DecodeToken(token);

            if (tokenData == null)
            {
                Logging.LogError("[NetworkManager] Provided token is invalid.");
                return;
            }

            _c.Get<ISessionData>().UserID = Guid.Parse(tokenData.sub);
            _c.Get<ISessionData>().Token = token;

            Logging.Log("Got userID:" + _c.Get<ISessionData>().UserID, LogMsgType.USER);
            Logging.Log("Got name: " + tokenData.name);
        }

        void OnReactPosBusConnected(string userId)
        {
            Logging.Log("[InitState] ReactPosBus Connected with userId: " + userId);

            _c.Get<ISessionData>().UserID = Guid.Parse(userId);
            _c.Get<IPosBus>().IsConnected = true;
            _c.Get<IPosBus>().ProcessMessageQueue = true;
        }

        public void Update() { }

        public void OnExit()
        {
            _c.Get<IReactPosBusClient>().OnPosBusConnected -= OnReactPosBusConnected;
        }

    }

}