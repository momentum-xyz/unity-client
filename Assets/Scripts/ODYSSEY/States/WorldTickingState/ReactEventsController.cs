using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using Cysharp.Threading.Tasks;

namespace Odyssey
{
    public class ReactEventsController : StateController
    {
        IReactBridge _reactBridge;
        public ReactEventsController(IMomentumContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            // React Bridge
            _reactBridge = _c.Get<IReactBridge>();

            _reactBridge.PauseUnity_Event += OnUnityClientPaused;
            _reactBridge.ResumeUnity_Event += OnUnityClientResumed;
            _reactBridge.ControlKeyboard_Event += OnControlKeyboardEvent;
            _reactBridge.ControllerSettings_Event += OnControllerSettingsReceived;
        }

        public override void OnExit()
        {
            _reactBridge.PauseUnity_Event -= OnUnityClientPaused;
            _reactBridge.ResumeUnity_Event -= OnUnityClientResumed;
            _reactBridge.ControlKeyboard_Event -= OnControlKeyboardEvent;
            _reactBridge.ControllerSettings_Event -= OnControllerSettingsReceived;
        }

        private void OnControllerSettingsReceived(string settingsStr)
        {
            var controllerSettings = JsonUtility.FromJson<HS.ControllerSettings>(settingsStr);
            _c.Get<ISessionData>().ControllerSettings = controllerSettings;
        }

        void OnControlKeyboardEvent(bool consumeClicks)
        {
            Logging.Log("[WorldTickingState] ControlKeyboard: " + consumeClicks);

            if (!consumeClicks)
            {
                WaitForKeyReleaseAndDisableInput(true).Forget();
            }
            else
            {
#if !UNITY_EDITOR && UNITY_WEBGL
                WebGLInput.captureAllKeyboardInput = true;
#endif
            }
        }

        void OnUnityClientResumed()
        {
            if (!_c.Get<ISessionData>().AppPaused) return;

            Logging.Log("[WorldTickingState] Received Unity Client Resume Event!");

            //Time.timeScale = 1f;
            _c.Get<ISessionData>().AppPaused = false;
            _c.Get<ILoadingScreenManager>().SetPaused(false);

            // Unpause the Avatar Controller
            if (_c.Get<ISessionData>().WorldAvatarController != null)
            {
                HS.ThirdPersonController controller = _c.Get<ISessionData>().WorldAvatarController.GetComponent<HS.ThirdPersonController>();
                controller.IsPaused = false;
            }

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = true;
#endif
            Logging.Log("[WorldTickingState] Enabled Keyboard Input");
        }

        void OnUnityClientPaused()
        {
            if (_c.Get<ISessionData>().AppPaused || !_c.Get<ISessionData>().WorldIsTicking) return;

            Logging.Log("[WorldTickingState] Received Unity Client Pause Event!");

            _c.Get<ISessionData>().AppPaused = true;
            _c.Get<ILoadingScreenManager>().SetPaused(true);

            // Pause the Avatar Controller
            if (_c.Get<HS.IThirdPersonController>() != null)
            {
                _c.Get<HS.IThirdPersonController>().IsPaused = true;
            }

            // stop keyboard input when paused
#if !UNITY_EDITOR && UNITY_WEBGL
             WaitForKeyReleaseAndDisableInput().Forget();
#endif

            //Time.timeScale = 0f;

        }

        async UniTask WaitForKeyReleaseAndDisableInput(bool force = false)
        {
            await UniTask.WaitUntil(() => (Input.anyKey == false));
            Logging.Log("[WorldTickingState] Disabling Unity Keyboard Input..");
#if !UNITY_EDITOR && UNITY_WEBGL
            // check if the app is still paused or we force the action
            if (_c.Get<ISessionData>().AppPaused || force) WebGLInput.captureAllKeyboardInput = false;
#endif
        }




    }
}