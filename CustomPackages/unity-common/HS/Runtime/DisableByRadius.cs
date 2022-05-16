using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class DisableByRadius : MonoBehaviour
	{
		[SerializeField] GameObject _targetObject;
		[SerializeField] float _radius = 100;
		[SerializeField] bool Invert;

		Transform _cam;

		void Update()
		{
			if( !_cam ) _cam = Camera.main?.transform;
			if( !_cam ) return;
			var d = (transform.position-_cam.position).sqrMagnitude;
			_targetObject.SetActive( Invert?!(d>_radius*_radius):(d>_radius*_radius) );
		}



		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere( transform.position, _radius );
		}
	}
}