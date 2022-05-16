using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HS
{
	public class AutoRotator : MonoBehaviour
	{
		public Vector3 Axis = new Vector3(0, 1, 0);
		public float Speed = 90;

		public float WanderFreq = 0;
		public float WanderSpeedAngular = 120f;

		public bool RandomStartRotation = false;

		const bool USEREALTIME = true;

		float _seed;

		void Awake()
		{
			_seed = Random.Range(0f, 1f);
		}

		void OnEnable()
		{
			if( RandomStartRotation ) transform.Rotate( Axis, Random.Range(0,360), Space.Self );
		}

		void Update()
		{
			float delta = USEREALTIME? Time.unscaledDeltaTime : Time.deltaTime;
			float time = USEREALTIME? Time.unscaledTime : Time.time;
			if (WanderFreq > 0)
			{
				Axis = Quaternion.Euler(
					new Vector3(
						Mathf.PerlinNoise(-90 * _seed, 12 + WanderFreq * time) * 2 - 1,
						Mathf.PerlinNoise(22 * _seed, -10.34f + WanderFreq * time) * 2 - 1,
						Mathf.PerlinNoise(5 * _seed, 88 + WanderFreq * time) * 2 - 1
					) * WanderSpeedAngular * delta
				) * Axis;
				Axis = Axis.normalized;
			}

			transform.Rotate(Axis, Speed * delta, Space.Self);
			// 	Axis += 
			// 		new Vector3(
			// 			Mathf.PerlinNoise( 12f, 12+WanderFreq*Time.deltaTime )*2-1,
			// 			Mathf.PerlinNoise( 234f, 0.2f+WanderFreq*Time.deltaTime )*2-1,
			// 			Mathf.PerlinNoise( -21f, 33-WanderFreq*Time.deltaTime )*2-1
			// 		);
			// 	Axis = Axis.normalized;
			// }
		}


		// was tryna make a gizmo-level preview of the rotation speed but failing
		// Vector3 _gizmoDir = Vector3.right;
		// void OnDrawGizmosSelected()
		// {
		// 	float newAngle = Time.unscaledDeltaTime *Speed;
		// 	_gizmoDir -= Vector3.Dot(_gizmoDir,Axis)*Axis;
		// 	_gizmoDir.Normalize();
		// 	_gizmoDir = Quaternion.AngleAxis(newAngle,Axis)*_gizmoDir;
		// 	Gizmos.matrix = transform.parent?transform.parent.localToWorldMatrix:Matrix4x4.identity	;
		// 	Gizmos.color = Color.cyan;
		// 	Gizmos.DrawRay(Vector3.zero,_gizmoDir);
		// 	// Gizmos.DrawRay(Vector3.zero, Axis * 0.2f);
		// }

	}
}