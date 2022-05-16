using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace HS
{
	public class AwardsManager : MonoBehaviour
	{
		static AwardsManager _instance;
		[SerializeField] GameObject _source;
		[SerializeField] Awards _awardsList;

		/// <summary> Spawns the award for the given team, and places it in the 
		/// given transform. Returns the award object. </summary>
		public static GameObject SetTeamAward( string team, Transform teamSpace )
		{
			if( !_instance ) return null;

			var def = _instance._awardsList.Find( team );
			if( def == null )
			{
				//Debug.Log( $"Can't find team [{team}] to give award display to." );
				return null;
			}

			var op = Instantiate( _instance._source );
			op.transform.SetParent( teamSpace );
			if( teamSpace ) op.transform.Zero();
			op.SetActive( true );

			var award = op.GetComponent<AwardDisplay>();
			award.Setup( def );

			return op;
		}


		public static GameObject RandomAward( Transform teamSpace )
		{
			if( !_instance ) return null;
			var teams = _instance._awardsList.List.Select( elm=>elm.Team ).ToList();
			var team = teams[Random.Range(0,teams.Count)];
			return SetTeamAward( team, teamSpace );
		}


		void Awake()
		{
			_instance = this;
			_source.SetActive( false );
		}
	}
}
