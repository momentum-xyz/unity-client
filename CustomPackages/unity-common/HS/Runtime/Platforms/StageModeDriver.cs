using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class StageModeDriver : MonoBehaviour
	{
		public bool ScaleToTetherRingRadius = true;
		public float ScaleReference = 5;


		void OnEnable()
		{
			Setup();
		}


		public void Setup()
		{
			if( !ScaleToTetherRingRadius ) return;
			
			var platform = GetComponentInParent<UserPlatformDriver>();
			if( !platform ) return;

			transform.localScale = Vector3.one * platform.TetherRingRadius/ScaleReference;
		}
	}
}