using Odyssey.Networking;
using Odyssey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace Odyssey
{
    public interface INetworkingService : IDisposable
    {
        public Action<string> OnConnectedToWorld_Event { get; set; }
        public Action OnDisconnected_Event { get; set; }

        public bool IsConnected { get; set; }

        public void Init(IMomentumContext context);
        public void InitNetworkingServices();
        public void ConnectServices();
        public void SetUserToken(string token);
        public void SetupFromConfig(NetworkingConfigData configData);
        public void SetupEventHandling();
        public void Update();
    }

    public class NetworkingSerivce : INetworkingService, IRequiresContext
    {
        private const float RECONNECT_TIMEOUT_SEC = 3.0f;
        public Action<string> OnConnectedToWorld_Event { get; set; }
        public Action OnDisconnected_Event { get; set; }

        public bool IsConnected { get { return _isConnected; } set { } }

        IMomentumContext _c;
        NetworkingConfigData _configData;
        IPosBus _posBus;
        bool _resendHandshakeOnConnect = false;
        bool _doReconnect = false;
        bool _afterReconnect = false;
        bool _ignorePositionMessages = false;
        bool _isConnected = false;
        float _reconnectTimer = 0.0f;


        public void Init(IMomentumContext context)
        {
            _c = context;
        }

        public void SetupFromConfig(NetworkingConfigData configData)
        {
            // Clone the original configuration and use the clone 
            // because if we modify something after that
            // that will modify the original data file, because it is a ScripableObject and it is Serialized
            NetworkingConfigData networkingConfigCopy = configData.Clone();

#if !UNITY_EDITOR && UNITY_WEBGL
        string domain = DataHelpers.GetDomainFromURL(Application.absoluteURL);
        
        // if we are running on localhost
        // overwrite the domain with one provided in the configuration
        if(domain.Contains("localhost") || domain.Contains("127.0.0.1")) {
            domain = configData.localDomainOverwrite;      
        }

        if(!networkingConfigCopy.ignoreApplicationURL)
            networkingConfigCopy.InitFromDomain(domain);
#endif
            _configData = networkingConfigCopy;

            networkingConfigCopy.AuthenticationToken = configData.AuthenticationToken;

            _c.Get<ISessionData>().NetworkingConfig = networkingConfigCopy;

            _c.Get<IBackendService>().APIEndpoint = _c.Get<ISessionData>().NetworkingConfig.apiEndpoint;
            _c.Get<IRendermanService>().RendermanEndpoint = _c.Get<ISessionData>().NetworkingConfig.rendermanURL;
            _c.Get<IRendermanService>().DefaultHash = _c.Get<ITextureService>().DefaultTextureHash;
        }

        public void SetupEventHandling()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            _c.Get<IReactBridge>().Token_Event += OnReceivedToken;
#endif
        }

        public void InitNetworkingServices()
        {
            _posBus = _c.Get<IPosBus>();

            _posBus.Init(_c.Get<ISessionData>().NetworkingConfig.posBusURL);

            _posBus.OnPosBusConnected += OnPosBusConnected;
            _posBus.OnPosBusDisconnected += OnPosBusDisconnected;
            _posBus.OnPosBusMessage += OnPosBusMessage;

            _afterReconnect = false;
            _doReconnect = false;
        }
        public void Dispose()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            _c.Get<IReactBridge>().Token_Event -= OnReceivedToken;
#endif
            _posBus.OnPosBusConnected -= OnPosBusConnected;
            _posBus.OnPosBusDisconnected -= OnPosBusDisconnected;
            _posBus.OnPosBusMessage -= OnPosBusMessage;

#if UNITY_EDITOR
            GameObject.DestroyImmediate(_configData, true);
#else
            GameObject.Destroy(_configData);
#endif
            _posBus.Disconnect();

            _afterReconnect = false;
            _doReconnect = false;
            _isConnected = false;
        }

        public void ConnectServices()
        {
            _posBus.SetToken(_c.Get<ISessionData>().Token, _c.Get<ISessionData>().UserID.ToString(), _c.Get<ISessionData>().SessionID);
            _posBus.Connect();
        }

        public void AuthenticatePosBus(bool sendDomain = true)
        {

            //Logging.Log("[NetworkingService] Sending handshake to posbus...");

            _posBus.SendHandshake
                (_c.Get<ISessionData>().Token,
                _c.Get<ISessionData>().UserID.ToString(),
                _c.Get<ISessionData>().SessionID,
                sendDomain ? _c.Get<ISessionData>().NetworkingConfig.domain : ""
                );
        }

        public void SetUserToken(string token)
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

            //Logging.Log("Got token: " + token, LogMsgType.USER);
            Logging.Log("Got userID:" + _c.Get<ISessionData>().UserID, LogMsgType.USER);
            Logging.Log("Got name: " + tokenData.name);
        }


        void OnPosBusConnected()
        {
            if (_isConnected) return;

            _isConnected = true;
            _posBus.ProcessMessageQueue = true;
            AuthenticatePosBus(!_resendHandshakeOnConnect);
            _resendHandshakeOnConnect = false;

        }

        /// <summary>
        /// !!! Note that the responses the this function are actually handled in the Update method
        /// because, if we do it here.. it will be executed on a separate thread and that causes problems
        /// </summary>
        /// <param name="errorCode"></param>
        void OnPosBusDisconnected(PosBusDisconnectError errorCode)
        {
            _isConnected = false;

            if (_c.Get<ISessionData>().IsUnityTerminatedExternaly)
            {
                Logging.Log("[NetworkingManager] PosBus Disconnected from an External Force!");
                _c.Get<ISessionData>().IsUnityTerminatedExternaly = false;
                return;
            }

            if (errorCode == PosBusDisconnectError.UNKNOWN)
            {
                Logging.Log("[NetworkingManager] PosBus disconnected due to unknown reason, trying to re-connect", LogMsgType.NETWORKING);

                DoReconnectAfterTime();
            }
        }

        void OnReceivedToken(string token)
        {
            if (_isConnected)
            {
                // if we are connected, just update the token we are using
                SetUserToken(token);
                _posBus.SetToken(_c.Get<ISessionData>().Token, _c.Get<ISessionData>().UserID.ToString(), _c.Get<ISessionData>().SessionID);
            }
            else
            {
                SetUserToken(token);
                InitNetworkingServices();
                ConnectServices();
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
                        if (_afterReconnect)
                        {
                            _afterReconnect = false;
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
            }
        }

        /// <summary>
        /// This function will force a reconnect, but after RECONNECT_TIMEOUT_SEC time seconds
        /// </summary>
        void DoReconnectAfterTime()
        {
            _resendHandshakeOnConnect = true;
            _afterReconnect = true;
            _doReconnect = true;
            _reconnectTimer = 0.0f;
        }

        public void Update()
        {
            if (_isConnected)
            {
                _posBus.ProcessReceivedMessagesFromMainThread();
            }

            if (_doReconnect)
            {
                _reconnectTimer += Time.deltaTime;

                if (_reconnectTimer >= RECONNECT_TIMEOUT_SEC)
                {
                    _doReconnect = false;
                    _posBus.Init(_c.Get<ISessionData>().NetworkingConfig.posBusURL);
                    _posBus.Connect();
                }
            }

        }
    }



}