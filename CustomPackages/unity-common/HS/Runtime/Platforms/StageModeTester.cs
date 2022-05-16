using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;


namespace HS
{
	public class StageModeTester : MonoBehaviour
	{
		public KeyCode TestKey = KeyCode.G;


		void Update()
		{
			if( Input.GetKeyDown(TestKey) )
			{
				var target = DoFindClosest();
				target?.SetStageMode( !target.StageMode );
			}
		}

		UserPlatformDriver DoFindClosest()
		{
			var coll = FindObjectsOfType<UserPlatformDriver>();
			UserPlatformDriver winner = null;
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
