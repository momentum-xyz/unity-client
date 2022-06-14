using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Odyssey.Networking;

namespace Odyssey
{
    public interface IUnityJSAPI : IRequiresContext
    {
    }

    public class UnityJSAPI : IUnityJSAPI
    {
        private IMomentumContext _c;

        public void Init(IMomentumContext context)
        {
            _c = context;
            NativeUnityJSAPI.Init(_c);
        }
    }

    public static class NativeUnityJSAPI
    {
        private static IMomentumContext _c;

#if UNITY_WEBGL && !UNITY_EDITOR
        public delegate void SetTokenCallback(IntPtr token);
        public delegate void PauseUnityCallback(int isPaused);
        public delegate void ControlSoundCallback(int isOn);
        public delegate void ControlVolumeCallback(int gain);
        public delegate void ControlKeyboardCallback(int unityIsInControl);
        public delegate void LookAtWispCallback(IntPtr userId);
        public delegate void ToggleMinimapCallback();
        public delegate string GetUserPositionCallback();
        public delegate string GetCurrentWorldCallback();
        public delegate void TeleportToSpaceCallback(IntPtr spaceGuid);
        public delegate void TeleportToUserCallback(IntPtr userGuid);
        public delegate void TeleportToVector3Callback(float x, float y, float z);
        public delegate void TriggerInteractionMsgCallback(int kind, IntPtr guid, int flag, IntPtr message);


        [DllImport("__Internal")]
        public static extern void SetCallbacks(
            GetUserPositionCallback getUserPositionPtr,
            GetCurrentWorldCallback getCurrentWorldPtr,
            SetTokenCallback setTokenPtr,
            PauseUnityCallback pauseUnityPtr,
            ControlSoundCallback controlSoundPtr,
            ControlVolumeCallback controlVolumePtr,
            ControlKeyboardCallback controlKeyboardPtr,
            LookAtWispCallback lookAtWispPtr,
            ToggleMinimapCallback toggleMinimapPtr,
            TeleportToSpaceCallback teleportToSpacePtr,
            TeleportToUserCallback teleportToUserPtr,
            TeleportToVector3Callback teleportToVectorPtr,
            TriggerInteractionMsgCallback triggerInteractionMsgPtr);
#endif

        public static void Init(IMomentumContext ctx)
        {
            _c = ctx;

#if UNITY_WEBGL && !UNITY_EDITOR
            SetCallbacks(
                GetUserPosition,
                GetCurrentWorld,
                SetToken,
                PauseUnity,
                ControlSound,
                ControlVolume,
                ControlKeyboard,
                LookAtWisp,
                ToggleMinimap,
                TeleportToSpace,
                TeleportToUser,
                TeleportToVector3,
                TriggerInteractionMsg);
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR

        [MonoPInvokeCallback(typeof(TriggerInteractionMsgCallback))]
        private static void TriggerInteractionMsg(int kind, IntPtr guid, int flag, IntPtr message)
        {
            string guidStr = Marshal.PtrToStringAuto(guid);
            string msgStr = Marshal.PtrToStringAuto(message);
            _c.Get<IPosBus>().TriggerInteractionMsg((uint)kind, Guid.Parse(guidStr), flag, msgStr);
        }

        [MonoPInvokeCallback(typeof(GetUserPositionCallback))]
        public static string GetUserPosition()
        {
            Vector3 playerPosition = _c.Get<ISessionData>().WorldAvatarController.transform.position;
            return playerPosition.ToString();
        }

        [MonoPInvokeCallback(typeof(GetUserPositionCallback))]
        public static string GetCurrentWorld()
        {
             return _c.Get<ISessionData>().WorldID.ToString();
        }

        [MonoPInvokeCallback(typeof(SetTokenCallback))]
        public static void SetToken(IntPtr token)
        {

        }

        [MonoPInvokeCallback(typeof(PauseUnityCallback))]
        public static void PauseUnity(int isPaused)
        {

        }

        [MonoPInvokeCallback(typeof(ControlSoundCallback))]
        public static void ControlSound(int isOn)
        {

        }

        [MonoPInvokeCallback(typeof(ControlVolumeCallback))]
        public static void ControlVolume(int gain)
        {

        }

        [MonoPInvokeCallback(typeof(ControlKeyboardCallback))]
        public static void ControlKeyboard(int unityIsInControl)
        {

        }

        [MonoPInvokeCallback(typeof(LookAtWispCallback))]
        public static void LookAtWisp(IntPtr userGuid)
        {

        }

        [MonoPInvokeCallback(typeof(ToggleMinimapCallback))]
        public static void ToggleMinimap()
        {

        }

        [MonoPInvokeCallback(typeof(TeleportToSpaceCallback))]
        public static void TeleportToSpace(IntPtr spaceGuid)
        {

        }

        [MonoPInvokeCallback(typeof(TeleportToUserCallback))]
        public static void TeleportToUser(IntPtr userGuid)
        {

        }
        [MonoPInvokeCallback(typeof(TeleportToVector3Callback))]
        public static void TeleportToVector3(float x, float y, float z)
        {

        }
#endif
    }

}
