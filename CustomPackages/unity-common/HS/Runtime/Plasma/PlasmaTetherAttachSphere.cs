using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class PlasmaTetherAttachSphere : MonoBehaviour, ITetherAttachShape
	{
		[SerializeField] float _radius;
		[SerializeField] bool _multiplyBySpaceRadius;

		UserPlatformDriver _driver;

		float _actualRadius => _radius * (_multiplyBySpaceRadius?_driver.TetherRingRadius:1);
		Transform _actualTransform => _multiplyBySpaceRadius?_driver.transform:transform;
		public Vector3 GetWorldPos( Vector3 origin ) =>
			_actualTransform.position+(origin-_actualTransform.position).normalized*_actualRadius;


		void Awake()
		{
			_driver = GetComponentInParent<UserPlatformDriver>();
		}

		void OnDrawGizmosSelected()
		{
			if( !_driver ) _driver = GetComponentInParent<UserPlatformDriver>();
			if( !_driver ) return;
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere( _actualTransform.position, _actualRadius );
		}
	}
}