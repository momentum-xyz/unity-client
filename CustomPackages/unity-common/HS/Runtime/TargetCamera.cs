using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
	public class TargetCamera : MonoBehaviour
	{
		public enum Method { Transform, LookAt, ActualTransformCopy, RespectYDirection }
		public Method Type = Method.Transform;

		public bool Flip;
		public bool Level;
		public bool IsAnimated; // if animated, we need to update even when the camera hasn't moved

        Vector3 _camCachePosition;
        Quaternion _camCacheRotation;
		Transform _cam;

		void LateUpdate()
		{
            if (!_cam)
            {
                Camera c = Camera.main;
                if (c == null) return; // this could happend, if we are switching worlds
                _cam = c.transform;
                _camCachePosition = new Vector3(0f, 0f, 0f);
                _camCacheRotation = new Quaternion(0f, 0f, 0f, 0f);
            }
			
            //if( !_cam ) return;       

            if (_cam.transform.position != _camCachePosition || _cam.transform.rotation != _camCacheRotation || IsAnimated )
            {
				if( Type == Method.RespectYDirection )
				{
					transform.rotation = Quaternion.LookRotation( transform.up, _cam.transform.position-transform.position );
					transform.rotation =
						Quaternion.AngleAxis( (Flip?-90:90), transform.right )
						*
						transform.rotation;
				}
                else if (Type != Method.ActualTransformCopy)
                {
                    var fwd = _cam.forward;
                    if (Type == Method.LookAt) fwd = _cam.position - transform.position;
                    if (Level) fwd.y = 0;
                    if (Flip) fwd = -fwd;
                    transform.rotation =
                        Quaternion.LookRotation(
                            fwd,
                            Vector3.up
                        );
                    // if( Flip ) transform.Rotate(Vector3.up, 180, Space.Self);
                }
                else
                {
                    transform.rotation = _cam.rotation;
                    // if( Flip ) 
                    // 	transform.Rotate( transform.up, 180, Space.Self );
                }

                _camCachePosition = _cam.transform.position;
                _camCacheRotation = _cam.transform.rotation;
            }
		}
	}
}