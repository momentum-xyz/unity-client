using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class AwardsTester : MonoBehaviour
	{
		public KeyCode RandomAwardKey = KeyCode.X;

		void Update()
		{
			if( Input.GetKeyDown( RandomAwardKey ) )
			{
				var teamSpaces = FindObjectsOfType<TeamSpaceDriver>();
				var team = teamSpaces[Random.Range(0,teamSpaces.Length)];
				GetAWowEvent.Create( team );
				AwardsManager.RandomAward( team.transform );
			}
		}
	}
}