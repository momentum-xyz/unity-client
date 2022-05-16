using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class ForceFieldDriver : MonoBehaviour
	{
		public bool ScaleToTetherRingRadius = true;

		void OnEnable()
		{
			Setup();
		}


		public void Setup()
		{
			if( !ScaleToTetherRingRadius ) return;
			
			var platformDriver = GetComponentInParent<UserPlatformDriver>();
			var tspaceDriver = GetComponentInParent<TeamSpaceDriver>();
			var trspaceDriver = GetComponentInParent<TrackSpaceDriver>();

			if( platformDriver != null )
				transform.localScale = Vector3.one*platformDriver.TetherRingRadius;
			else if( tspaceDriver != null )
				transform.localScale = Vector3.one*tspaceDriver.TetherRingRadius;
			else if( trspaceDriver != null )
				transform.localScale = Vector3.one*trspaceDriver.TetherRingRadius;
		}
	}
}