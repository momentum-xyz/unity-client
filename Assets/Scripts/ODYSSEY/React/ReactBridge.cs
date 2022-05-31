using UnityEngine;
using System;
using Odyssey.Networking;

namespace Odyssey
{
    public interface IReactBridge
    {
        public Action ToggleAllSound_Event { get; set; }
        public Action TurnAllSoundOn_Event { get; set; }
        public Action TurnAllSoundOff_Event { get; set; }
        public Action<string> OnSetVolume_Event { get; set; }
        public Action<string> LookAtWisp_Event { get; set; }
        public Action PauseUnity_Event { get; set; }
        public Action ResumeUnity_Event { get; set; }
        public Action<string> TeleportToSpace_Event { get; set; }
        public Action<Vector3> TeleportToPosition_Event { get; set; }
        public Action<string> TeleportToUser_Event { get; set; }
        public Action ToggleMinimap_Event { get; set; }
        public Action<bool> ControlKeyboard_Event { get; set; }
        public Action<Vector3, int> GoToWaypoint_Event { get; set; }
        public Action CancelGoToWaypoint_Event { get; set; }
        public Action<string> ControllerSettings_Event { get; set; }
    }

    /// <summary>
    /// !!! The only place to define functions called from the React Layer !!!
    /// Every function will need to pair with an event that all interested parties will subscribe to
    /// This Component should be attached only on a GameObject named "UnityManager", because React will "SendMessage" to this GameObject
    /// </summary>
    public class ReactBridge : MonoBehaviour, IRequiresContext, IReactBridge
    {
        public Action ToggleAllSound_Event { get; set; }
        public Action TurnAllSoundOn_Event { get; set; }
        public Action TurnAllSoundOff_Event { get; set; }
        public Action<string> OnSetVolume_Event { get; set; }
        public Action<string> LookAtWisp_Event { get; set; }
        public Action PauseUnity_Event { get; set; }
        public Action ResumeUnity_Event { get; set; }
        public Action<string> TeleportToSpace_Event { get; set; }
        public Action<Vector3> TeleportToPosition_Event { get; set; }
        public Action<string> TeleportToUser_Event { get; set; }
        public Action ToggleMinimap_Event { get; set; }
        public Action<bool> ControlKeyboard_Event { get; set; }
        public Action<Vector3, int> GoToWaypoint_Event { get; set; }
        public Action CancelGoToWaypoint_Event { get; set; }
        public Action<string> ControllerSettings_Event { get; set; }

        IMomentumContext _c;

        public void Init(IMomentumContext context)
        {
            this._c = context;
        }


        #region Functions Called From React
        public void toggleAllSound()
        {
            ToggleAllSound_Event?.Invoke();
        }

        public void turnAllSoundOn()
        {
            TurnAllSoundOn_Event?.Invoke();
        }

        public void setVolume(string volumeString)
        {
            OnSetVolume_Event?.Invoke(volumeString);
        }

        public void toggleMinimap()
        {
            ToggleMinimap_Event?.Invoke();
        }

        public void turnAllSoundOff()
        {
            TurnAllSoundOff_Event?.Invoke();
        }

        public void lookAtWisp(string wispGuid)
        {
            LookAtWisp_Event?.Invoke(wispGuid);
        }

        public void setToken(string token)
        {
            Debug.Log("Got token from React: " + token);
            //      Token_Event?.Invoke(token);
        }
        public void setPosbusURL(string url)
        {
            _c.Get<ISessionData>().NetworkingConfig.posBusURL = url;
        }

        public void setAddressablesURL(string url)
        {
            _c.Get<ISessionData>().NetworkingConfig.addressablesURL = url;
        }

        public void setOverwriteDomain(string domain)
        {
            _c.Get<ISessionData>().NetworkingConfig.localDomainOverwrite = domain;
            _c.Get<ISessionData>().NetworkingConfig.InitFromDomain(domain);
            _c.Get<IBackendService>().APIEndpoint = _c.Get<ISessionData>().NetworkingConfig.apiEndpoint;
            _c.Get<IRendermanService>().RendermanEndpoint = _c.Get<ISessionData>().NetworkingConfig.rendermanURL;
            _c.Get<IRendermanService>().DefaultHash = _c.Get<ITextureService>().DefaultTextureHash;
        }

        public void pauseUnityClient()
        {
            PauseUnity_Event?.Invoke();
        }

        public void resumeUnityClient()
        {
            ResumeUnity_Event?.Invoke();
        }

        public void controlKeyboard(int consumeClicks)
        {
            ControlKeyboard_Event?.Invoke(consumeClicks > 0);
        }

        public void teleportToSpace(string spaceUid)
        {
            Debug.Log("Teleport To Space: " + spaceUid);
            TeleportToSpace_Event?.Invoke(spaceUid);
        }


        public void teleportToVector3(Vector3 destination)
        {
            Debug.Log("Teleport To Vector3: " + destination);
            TeleportToPosition_Event?.Invoke(destination);
        }

        public void teleportToVector3(string message)
        {
            Debug.Log("Got teleport from React to location for " + message);
            Vector3 destination = getVector3(message, 1, message.Length - 2);
            TeleportToPosition_Event?.Invoke(destination);
        }

        public void teleportToUser(string guid)
        {
            Debug.Log("Teleporting to user: " + guid);
            TeleportToUser_Event?.Invoke(guid);
        }

        // format is (x,y,z)[index]
        public void goToWaypoint(string message)
        {
            Debug.Log("Got go to waypoint from React to location for " + message);
            var split = message.Split('[');
            Vector3 destination = getVector3(message, 1, split[0].Length - 2);
            var index = int.Parse(split[1].Substring(0, split[1].Length - 2));
            GoToWaypoint_Event?.Invoke(destination, index);
        }

        public void cancelGoToWaypoint()
        {
            CancelGoToWaypoint_Event?.Invoke();
        }

        public void setControllerSettings(string settingsStr)
        {
            ControllerSettings_Event?.Invoke(settingsStr);
        }

        Vector3 getVector3(string rString, int startIndex, int endIndex)
        {
            string[] temp = rString.Substring(startIndex, endIndex).Split(',');
            float x = float.Parse(temp[0]);
            float y = float.Parse(temp[1]);
            float z = float.Parse(temp[2]);
            Vector3 rValue = new Vector3(x, y, z);
            return rValue;
        }
        #endregion
    }
}