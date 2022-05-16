using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class PlasmaTetherTester : MonoBehaviour
	{
		public KeyCode RandomTetherKey = KeyCode.T;

		void Update()
		{
			if( Input.GetKeyDown( RandomTetherKey ) )
			{
				var teamSpaces = FindObjectsOfType<TeamSpaceDriver>();
				var team1 = teamSpaces[Random.Range(0,teamSpaces.Length)];
				var team2 = teamSpaces[Random.Range(0,teamSpaces.Length)];
				PlasmaTetherSolo.Create( team1.transform, team2.transform, true );
			}
		}
	}
}