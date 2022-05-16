using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	/// <summary>
	/// Allows creator to define a more refined tether attach shape. Can be placed on a child
	/// object of a UserPlatformDriver.
	/// </summary>
	public class PlasmaTetherAttachShape : MonoBehaviour, ITetherAttachShape
	{
		[SerializeField] AnimationCurve _envelope = new AnimationCurve( new Keyframe(0,0.5f), new Keyframe( 1, 0.5f ) );
		[SerializeField] [Range(0,360)] float _angle;
		[SerializeField] [Range(12,72)] int _gizmoSteps = 12;

		UserPlatformDriver _driver;


		public Vector3 GetWorldPos( Vector3 origin )
		{
			var dir = transform.InverseTransformPoint(origin).normalized;
			dir.y = 0;
			dir = dir.normalized;
			var originAngle = 
				Vector3.SignedAngle(
					Vector3.forward,
					dir,
					Vector3.up
				)
				+360-_angle;
			originAngle %= 360;
			return
				transform.TransformPoint(
					dir 
					*( _driver ? _driver.TetherRingRadius : 12 )
					*2
					*_envelope.Evaluate( originAngle/360f )
				);
		}


		void Awake()
		{
			_driver = GetComponentInParent<UserPlatformDriver>();
		}



		void OnDrawGizmosSelected()
		{
			if( !_driver ) _driver = GetComponentInParent<UserPlatformDriver>();
			if( !_driver ) return;

			Gizmos.color = Color.cyan;
			Gizmos.matrix = transform.localToWorldMatrix;

			var radius = _driver.TetherRingRadius;

			var dir = Quaternion.AngleAxis( _angle, Vector3.up ) * Vector3.forward;
			var lastPos = dir;
			for( int i = 0; i < _gizmoSteps+1; i++ )
			{
				var ph = (float)i/(float)_gizmoSteps;
				var pos = dir *radius *2 *_envelope.Evaluate(ph);
				if( i>0 ) Gizmos.DrawLine( Vector3.zero, pos );
				if( i>0 ) Gizmos.DrawLine( pos, lastPos );
				lastPos = pos;
				dir = Quaternion.AngleAxis( 360f/(float)_gizmoSteps, Vector3.up ) *dir;
				dir = dir.normalized;
			}
		}
	}
}