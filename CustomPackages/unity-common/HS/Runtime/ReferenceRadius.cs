using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class ReferenceRadius : MonoBehaviour
	{
		public float Radius = 1;
		public bool DrawAlways = true;


		void OnDrawGizmos()
		{
			if( !DrawAlways ) return;
			Gizmos.color = Color.cyan;
			NHelp.DrawGizmosCircle( transform.position, Vector3.up, Radius );
		}


		void OnDrawGizmosSelected()
		{
			if( DrawAlways ) return;
			Gizmos.color = Color.cyan;
			NHelp.DrawGizmosCircle( transform.position, Vector3.up, Radius );
		}
	}
}