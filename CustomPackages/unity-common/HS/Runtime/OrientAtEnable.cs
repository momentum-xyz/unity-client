using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class OrientAtEnable : MonoBehaviour
	{
		public Vector3 WorldDirForZ = Vector3.forward;
		public float Delay = 0;


		void OnEnable()
		{
			if( Delay == 0 ) transform.rotation = Quaternion.LookRotation( WorldDirForZ );
			else StartCoroutine( DelayedOrient() );
		}

		IEnumerator DelayedOrient()
		{
			yield return new WaitForSeconds( Delay );
			transform.rotation = Quaternion.LookRotation( WorldDirForZ );
		}
	}
}