using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Nextensions;


namespace HS
{
	public class PlatformLayoutTester : MonoBehaviour
	{
		[Header( "Generation" )]
		public bool GenerateSpaces;
		public GameObject ProgramSpacePrefab;
		public GameObject ChallengeSpacePrefab;
		public GameObject TeamSpacePrefab;
		IPlatformLayouter _layouter;


		[Header( "Automation" )]
		public bool AutoFind = false;
		public string ProgramString = "PROGRAMSPACE";
		public string ChallengeString = "CHALLENGESPACE";
		public string TeamString = "TEAMSPACE";

		[Header( "Definition" )]
		public List<UserPlatformDriver> ProgramSpaces;
		[FormerlySerializedAs( "ProjectSpaces" )]
		public List<UserPlatformDriver> ChallengeSpaces;
		public List<UserPlatformDriver> TeamSpaces;


		void OnEnable()
		{
			if( GenerateSpaces )
				Generate();

			if( AutoFind )
			{
				ProgramSpaces.Clear();
				ChallengeSpaces.Clear();
				TeamSpaces.Clear();
				foreach( var p in FindObjectsOfType<UserPlatformDriver>() )
				{
					// Debug.Log( $"Trying {p.name}" );
						 if( p.name.ToUpper().Contains( ProgramString.ToUpper() ) ) ProgramSpaces.Add( p );
					else if( p.name.ToUpper().Contains( ChallengeString.ToUpper() ) ) ChallengeSpaces.Add( p ); 
					else if( p.name.ToUpper().Contains( TeamString.ToUpper() ) ) TeamSpaces.Add( p ); 
				}
			}

			StartCoroutine( DelayedAttach() );
		}

		IEnumerator DelayedAttach()
		{
			yield return new WaitForSeconds( 0.2f );
			foreach( var op in ChallengeSpaces )
				op.SetParent( Closest( op, ProgramSpaces) );
			foreach( var op in TeamSpaces )
				op.SetParent( Closest( op, ChallengeSpaces ) );
			foreach( var op in ProgramSpaces )
				op.SetParent( (UserPlatformDriver)null );
		}


		[ContextMenu( "Generate" )]
		void Generate()
		{
			if( Application.isPlaying == false ) return;
			if( _layouter == null ) _layouter = GetComponent<IPlatformLayouter>();
			if( _layouter == null )
			{
				Debug.Log( "WARNING: no layouter found to generate spaces. Please add one." );
				return;
			}
			_layouter.Generate( ProgramSpacePrefab, ChallengeSpacePrefab, TeamSpacePrefab );
		}


		UserPlatformDriver Closest( UserPlatformDriver location, List<UserPlatformDriver> others )
		{
			UserPlatformDriver winner = null;
			float winDst = Mathf.Infinity;
			foreach( var o in others )
			{
				var dst = (location.transform.position-o.transform.position).sqrMagnitude;
				if( dst < winDst )
				{
					winDst = dst;
					winner = o;
				}
			}
			return winner;
		}


		void OnDrawGizmosSelected()
		{
			if( !GenerateSpaces ) return;
			if( _layouter == null ) _layouter = GetComponent<IPlatformLayouter>();
			if( _layouter == null ) return;
			_layouter.DrawGizmos();
		}
	}
}