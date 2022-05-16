using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS
{
    /// <summary> Manages the global volume. Already takes Globals.paused into account.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary> Meant for an absolute volume (0:1 range) from user-side.</summary>
        public static void SetGlobalVolume(float newVolume) => _globalVolume = newVolume;
        /// <summary> Meant for an absolute mute from user-side. HS.AudioManager already
        /// takes the state of Globals.paused into account. </summary>
        public static void SetMute(bool mute) => _globalMute = mute;

        [SerializeField] float _baseVolume = 1;
        [SerializeField] float _pauseVolumeFactor = 0.25f;

        static float _globalVolume = 1;
        static bool _globalMute = false;

        void Update()
        {
            AudioListener.volume =
                Mathf.Clamp01(
                    Mathf.Lerp(
                        AudioListener.volume
                        ,
                        (Time.timeScale == 0 ? _pauseVolumeFactor : 1)
                            * (_globalMute ? 0 : 1)
                            * _baseVolume
                            * _globalVolume
                        ,
                        Time.unscaledDeltaTime * 12f                    // MAGIC
                    )
                )
            ;
        }
    }
}