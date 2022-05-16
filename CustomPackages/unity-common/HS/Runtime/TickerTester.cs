using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class TickerTester : MonoBehaviour
	{
		IEnumerator Start()
		{
			var driver = GetComponentInChildren<TickerDriver>();
			while( true )
			{
				yield return new WaitForSecondsRealtime( 1 );
				driver.SetTopTicker( $"{Random.Range(0,24):00}:{Random.Range(0,60):00}:{Random.Range(0,60):00}" );
				driver.SetBottomTicker( $"{Random.Range(0,24):00}:{Random.Range(0,60):00}:{Random.Range(0,60):00}" );
				driver.SetTopLabel( new[]{"Waiting for launch!","End Of Hackathon!","Some other way too long label!"}.One() );
				driver.SetBottomLabel( new[]{"Waiting for Launch","Arrival","Show time! Go go go","We've got this! Or do we?","Challenges and temptations","Abyss","Transformation","Gogogo all out","Hurry up and finish","Time is up","Some other unwieldly long label that won't ever fit!"}.One() );
			}
		}
	}
}