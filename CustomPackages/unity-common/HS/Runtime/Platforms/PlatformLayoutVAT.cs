using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;

namespace HS
{
	public class PlatformLayoutVAT : MonoBehaviour, IPlatformLayouter
	{
		[Header( "Program Spaces Spiral Settings" )]
		public int ProgramSpaceCount = 4;
		public float ProgramSpiralRadius = 800;
		public float ProgramSpaceHeight = 52;
		[Header( "Challenge Valley Circle Settings" )]
		public float ChallengeSpaceRadius = 200;
		public Vector2 MinMaxChallengeSpaceCount = new Vector2(1,5);
		public bool RandomChallengeRotation = true;
		[Header( "Team Space Helix Settings" )]
		public Vector2 MinMaxTeamSpaceCount = new Vector2( 2, 15 );
		public float TeamSpacesPerTurn = 4;
		public float TeamSpaceHeight = 50;
		public float TeamSpaceGrowth = 50;
		public float TeamSpaceRadius1 = 150;
		public float TeamSpaceRadius2 = 200;

		HashSet<GameObject> _progs = new HashSet<GameObject>();
		HashSet<GameObject> _chals = new HashSet<GameObject>();
		HashSet<GameObject> _teams = new HashSet<GameObject>();

		public void Generate( GameObject programSpacePrefab, GameObject challengeSpacePrefab, GameObject teamSpacePrefab )
		{
			foreach( var p in _progs ) Destroy( p ); _progs.Clear();
			foreach( var c in _chals ) Destroy( c ); _chals.Clear();
			foreach( var t in _teams ) Destroy( t ); _teams.Clear();
			for( int i = 0; i < ProgramSpaceCount; i++ )
			{
				var pop = programSpacePrefab.Spawn();
				var pPos = Spiral( i, ProgramSpiralRadius );
				pop.transform.position = pPos + Vector3.up*ProgramSpaceHeight;
				_progs.Add( pop );
				var ccnt = (int)Random.Range(MinMaxChallengeSpaceCount.x,MinMaxChallengeSpaceCount.y);
				for( int j = 0; j < ccnt; j++ )
				{
					var cop = challengeSpacePrefab.Spawn();
					cop.transform.position = pPos + Circle( (float)j/(float)ccnt, ChallengeSpaceRadius );
					if( RandomChallengeRotation )
						cop.transform.rotation = Quaternion.AngleAxis(Random.Range(0,360),Vector3.up);
					else
						cop.transform.rotation = Quaternion.LookRotation(Vector3.Scale(pop.transform.position-cop.transform.position,new Vector3(1,0,1)),Vector3.up);
					_chals.Add(cop);
					var tcnt = Random.Range( MinMaxTeamSpaceCount.x, MinMaxTeamSpaceCount.y );
					for( int k = 0; k < tcnt; k++ )
					{
						var top = teamSpacePrefab.Spawn();
						top.transform.position = cop.transform.position + Vector3.up*TeamSpaceHeight +Helix( k/TeamSpacesPerTurn, TeamSpaceRadius1, TeamSpaceRadius2, TeamSpaceGrowth );
						top.transform.rotation = Quaternion.LookRotation(Vector3.Scale(cop.transform.position-top.transform.position,new Vector3(1,0,1)),Vector3.up);	
						_teams.Add(top);
					}
				}
			}
		}


		public void DrawGizmos()
		{
			for( int idx = 1; idx < ProgramSpaceCount; idx++ )
				Gizmos.DrawLine( Spiral(idx,ProgramSpiralRadius) , Spiral(idx-1,ProgramSpiralRadius) );

			for( int tidx = 1; tidx < 15; tidx ++ )
				Gizmos.DrawLine(
					Vector3.up*TeamSpaceHeight
						+
						Helix( (tidx-1)/TeamSpacesPerTurn, TeamSpaceRadius1, TeamSpaceRadius2, TeamSpaceGrowth )
					,
					Vector3.up*TeamSpaceHeight
						+
						Helix( (tidx)/TeamSpacesPerTurn, TeamSpaceRadius1, TeamSpaceRadius2, TeamSpaceGrowth )
				);
		}



		Vector3 Spiral( int idx, float rad = 200 )
		{
			if( idx <= 0 ) return Vector3.zero;
			return
				Quaternion.AngleAxis( (idx-1f)/4f *360f, Vector3.up )
				* Vector3.forward
				* (rad*  (int)((idx-1f)/4f+1f) );
		}

		Vector3 Circle( float phase, float rad )
		{
			return
				Quaternion.AngleAxis( phase*360, Vector3.up )
				*Vector3.forward
				*rad;

			// return Vector3.zero;
		}

		Vector3 Helix( float phase, float rad1, float rad2, float up )
		{
			return
				up*phase*Vector3.up
				+
				Quaternion.AngleAxis( phase*360, Vector3.up )
				* Vector3.forward
				* (
					rad1
					+
					phase*(rad2-rad1)
				);


			return Vector3.zero;
		}
	}
}