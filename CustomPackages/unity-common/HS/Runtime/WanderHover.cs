using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
	public class WanderHover : MonoBehaviour
	{
		public float Radius = 0.09f;
		public float RadiSpeed = 110f;

		public float WanderSpeedAngular = 60f;
		public float WanderFreq = 0.5f;


		Vector3 _axis = Vector3.up;
		float _phase;

		float _seed;


		void Awake()
		{
			_seed = Random.Range( 0f, 1f );
			_phase = _seed;
		}

		void Update()
		{
			_axis = Quaternion.Euler(
				new Vector3(
					Mathf.PerlinNoise( -90*_seed, 12 +WanderFreq*Time.time ) *2-1,
					Mathf.PerlinNoise( 22*_seed, -10.34f +WanderFreq*Time.time ) *2-1,
					Mathf.PerlinNoise( 5, 88*_seed +WanderFreq*Time.time ) *2-1
				) *WanderSpeedAngular *Time.deltaTime
			) * _axis;


			_phase += Time.deltaTime * (RadiSpeed/360f);
			_phase %= 1;
			Vector3 lPos = Vector3.right *Radius;
			lPos = Quaternion.AngleAxis( _phase * 360, Vector3.up ) *lPos;
			lPos = Quaternion.FromToRotation( Vector3.up, _axis ) *lPos;
			transform.localPosition = lPos;


		}


		// void OnDrawGizmosSelected()
		// {
		// 	Gizmos.matrix = transform.parent.localToWorldMatrix;
		// 	Gizmos.color = Color.red;
		// 	Gizmos.DrawRay( Vector3.zero, _axis );
		// }

	}
}