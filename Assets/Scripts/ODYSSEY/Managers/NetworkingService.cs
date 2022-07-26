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
        public Action<string> OnConnectedToWorld_Event { get; set; }
        public Action OnDisconnected_Event { get; set; }

        public bool IsConnected { get { return _isConnected; } set { } }

        IMomentumContext _c;
        NetworkingConfigData _configData;
        IPosBus _posBus;
        bool _resendHandshakeOnConnect = false;
        bool _doReconnect = false;
        bool _isConnected = false;

        bool _isInit = false;

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
            _c.Get<IUnityJSAPI>().Token_Event -= OnReceivedToken;
            _c.Get<IUnityJSAPI>().Token_Event += OnReceivedToken;

        }

        public void InitNetworkingServices()
        {

            if (_isInit) return;

            _posBus = _c.Get<IPosBus>();

            _posBus.Init(_c.Get<ISessionData>().NetworkingConfig.posBusURL);

            _posBus.OnPosBusConnected -= OnPosBusConnected;
            _posBus.OnPosBusDisconnected -= OnPosBusDisconnected;
            _posBus.OnPosBusMessage -= OnPosBusMessage;

            _posBus.OnPosBusConnected += OnPosBusConnected;
            _posBus.OnPosBusDisconnected += OnPosBusDisconnected;
            _posBus.OnPosBusMessage += OnPosBusMessage;

            _c.Get<IPosBus>().HasReconnected = false;
            _doReconnect = false;
            _isInit = true;

        }
        public void Dispose()
        {

#if UNITY_EDITOR
            GameObject.DestroyImmediate(_configData, true);
#else
            GameObject.Destroy(_configData);
#endif
            _posBus.Disconnect();

            _c.Get<IPosBus>().HasReconnected = false;
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

            _c.Get<IPosBus>().TokenIsNotValid = false;
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

            if (errorCode == PosBusDisconnectError.UNKNOWN && _c.Get<IPosBus>().TokenIsNotValid)
            {
                Logging.Log("[NetworkingManager] Our token is invalid, so do not re-connect until we receive a new token.");
                return;
            }

            if (_c.Get<ISessionData>().IsUnityTerminatedExternaly)
            {
                Logging.Log("[NetworkingManager] PosBus Disconnected from an External Force!");
                _c.Get<ISessionData>().IsUnityTerminatedExternaly = false;
                return;
            }

            if (errorCode == PosBusDisconnectError.UNKNOWN)
            {
                Logging.Log("[NetworkingManager] PosBus disconnected due to unknown reason, trying to re-connect", LogMsgType.NETWORKING);

                DoReconnect();
                return;
            }
        }

        void OnReceivedToken(string token)
        {
            SetUserToken(token);

            _posBus.SetToken(_c.Get<ISessionData>().Token, _c.Get<ISessionData>().UserID.ToString(), _c.Get<ISessionData>().SessionID);

            if (!_isConnected)
            {
                ConnectServices();
            }

        }

        void OnPosBusMessage(IPosBusMessage msg)
        {

        }

        /// <summary>
        /// This function will force a reconnect
        /// </summary>
        void DoReconnect()
        {
            _resendHandshakeOnConnect = true;
            _c.Get<IPosBus>().HasReconnected = true;
            _doReconnect = true;
        }



        public void Update()
        {
            if (_doReconnect)
            {
                _doReconnect = false;
                _posBus.Init(_c.Get<ISessionData>().NetworkingConfig.posBusURL);
                _posBus.Connect();
            }

        }
    }



}