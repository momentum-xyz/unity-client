using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.Universal;


namespace HS
{
    public class QualitySetter : MonoBehaviour
    {
        public enum Quality{High,Low}
        static Quality _quality;
        public static Quality CurrentQuality{get{return _quality;}set{SetQuality(value);}}

        public static void SetQuality( Quality newQuality )
        {
            //var cam = Camera.main;
            //var data = null; //cam?.GetComponent<UniversalAdditionalCameraData>();
            //if( data != null )
            //{
            //	data.renderShadows = newQuality==Quality.High;
            //}
            //_quality = newQuality;
        }

        [SerializeField] KeyCode _toggleShadows = KeyCode.Backslash;
        void Update()
        {
            if( Application.isEditor )
            {
                if( Input.GetKeyDown( _toggleShadows) )
                {
                    SetQuality( _quality==Quality.High?Quality.Low:Quality.High );
                }
            }
        }
    }
}
