using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;


namespace HS
{
	public class ForceFieldTester : MonoBehaviour
	{
		public KeyCode TestKey = KeyCode.F;
		public bool FindClosest = false;
		// [Tooltip( "Leave empty to automatically find UserPlatformDrivers and toggle their forcefield." )]
		// public GameObject ForceFieldSource;


		void Update()
		{
			if( Input.GetKeyDown(TestKey) )
			{
				var target = 
					FindClosest
						? DoFindClosest()
						: FindObjectsOfType<UserPlatformDriver>().PickOne();

				target.SetPrivacy( !target.Privacy, target.Privacy );
			}
		}

		UserPlatformDriver DoFindClosest()
		{
			var coll = FindObjectsOfType<UserPlatformDriver>();
			UserPlatformDriver winner = coll[0];
			float winDst = Mathf.Infinity;
			foreach( var op in coll )
			{ 
				var dst = (op.transform.position-Camera.main.transform.position).sqrMagnitude;
				if( dst < winDst )
				{
					winDst = dst;
					winner = op;
				}
			}
			return winner;
		}
	}
}
