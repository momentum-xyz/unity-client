using HS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Odyssey
{

    public interface ISessionData
    {
        public bool WorldIsTicking { get; set; }
        public bool IsUnityTerminatedExternaly { get; set; }
        public GameObject WorldAvatarController { get; set; }
        public Camera AvatarCamera { get; set; }
        public NetworkingConfigData NetworkingConfig { get; set; }
        public WorldDefinition worldDefinition { get; set; }
        public string Token { get; set; }
        public Guid UserID { get; set; }
        public string SessionID { get; set; }
        public string WorldID { get; set; }
        public string LastWorldID { get; set; }
        public bool MutedSound { get; set; }
        public string SoundVolume { get; set; }
        public bool GotSelfPositionMsg { get; set; }
        public Vector3 SelfPosition { get; set; }
        public ControllerSettings ControllerSettings { get; set; }
        public bool AppPaused { get; set; }

        public void ParseToken(string token);
    }

    /// <summary>
    /// Holds all data related to the Current user session
    /// </summary>
    public class SessionData : ISessionData
    {
        // Check if we have an active world
        public bool WorldIsTicking { get; set; }


        public NetworkingConfigData NetworkingConfig
        {
            get; set;
        }

        public string Token { get; set; } = "";
        public Guid UserID { get; set; } = Guid.Empty;
        public string SessionID { get; set; } = "";
        public string WorldID { get; set; } = "";
        public string LastWorldID { get; set; } = "";

        public bool MutedSound { get; set; }
        public string SoundVolume { get; set; } = "1";

        // This will be set to true, if we receive 
        // a Signal=1 from PosBus, usually happens when we open a client twice

        public bool IsUnityTerminatedExternaly { get; set; }

        public GameObject WorldAvatarController { get; set; }
        public Camera AvatarCamera { get; set; }

        public WorldDefinition worldDefinition { get; set; }

        public bool GotSelfPositionMsg { get; set; }
        public Vector3 SelfPosition { get; set; }

        public bool AppPaused { get; set; } = false;

        ControllerSettings _controllerSettings;
        public ControllerSettings ControllerSettings
        {
            get => _controllerSettings;
            set
            {
                _controllerSettings = value;

                var avatarControllerGo = WorldAvatarController;

                if (avatarControllerGo != null)
                    avatarControllerGo.GetComponent<ThirdPersonController>().Settings = value;
            }
        }

        /// <summary>
        /// Parse the authentication token received from React and store it in the SessionData
        /// </summary>
        /// <param name="token"></param>
        public void ParseToken(string token)
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

            UserID = Guid.Parse(tokenData.sub);
            Token = token;

            //Logging.Log("Got token: " + token, LogMsgType.USER);
            Logging.Log("Got userID:" + UserID, LogMsgType.USER);
            Logging.Log("Got name: " + tokenData.name);
        }
    }

}
