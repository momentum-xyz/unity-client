using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
	public class ArrangeSpacesInteractively : MonoBehaviour
	{
		public bool IsLive = false;
		[Header( "Layout" )]
		[Range(1,10)]   public float EarthScale = 3.2f;

		[Range(0,100)]  public float MainHeight = 20;
		[Range(50,400)] public float MainRadius = 200;

		[Range(0,400)]  public float MainTetherRadius = 283;

		[Range(0,40)]   public float TeamsRadius = 20;
		[Range(0,20)]   public float TeamsHeight = 12;

		[Header( "DreamFlight Tweaks" )]
		[Range(-180,180)] public float TeamRotOffset;
		[Range(0,12)] public float HeightVar;


		[Header( "Refs" )]
		public EarthAndRingsDriver Earth;
		public TrackSpaceDriver JediLounge;
		public TrackSpaceDriver MissionControl; 


		[Header( "System" )]
		public LODTester TeamSpaceLODTester;


		public bool DidArrange{get;private set;}
		public List<Transform> GetTeamTotemLocations()
			=> _trackSpaces
				.Where( elm=>elm!=JediLounge&&elm!=MissionControl )
				.Select( elm=>elm.transform )
				.ToList();


		List<TrackSpaceDriver> _trackSpaces;
		Dictionary<TrackSpaceDriver,HashSet<TeamSpaceDriver>> _teamSpaces = new Dictionary<TrackSpaceDriver, HashSet<TeamSpaceDriver>>();


		void Start()
		{
			_trackSpaces = new List<TrackSpaceDriver>( GetComponentsInChildren<TrackSpaceDriver>() );
			foreach( var track in _trackSpaces )
			{
				_teamSpaces.Add( track, new HashSet<TeamSpaceDriver>() );
			}
			if( JediLounge != null ) _teamSpaces.Add( JediLounge, new HashSet<TeamSpaceDriver>() );
			if( MissionControl != null ) _teamSpaces.Add( MissionControl, new HashSet<TeamSpaceDriver>() );
			foreach( var team in GetComponentsInChildren<TeamSpaceDriver>() )
				_teamSpaces[team.TestTrackSpace].Add( team );

			DuplicateTeamSpaces();
			
			// yield return null;
			if( JediLounge )
			{
				int cnt = 0;
				foreach( var space in _teamSpaces[JediLounge] )
				{
					// Debug.Log( $"space {space} getting partner {(JediTeamSpaceDriver.Partner)cnt}" );
					space.GetComponent<JediTeamSpaceDriver>()?.SetupTeamPartner((JediTeamSpaceDriver.Partner)cnt);
					cnt++;
				}
			}

			// if( TeamSpaceLODTester )
			// {
			// 	var spaces = new HashSet<LODSet>();
			// 	foreach( var grp in _teamSpaces.Values )
			// 		foreach( var spc in grp )
			// 			spaces.Add( spc.GetComponent<LODSet>() );
			// 	TeamSpaceLODTester.Setters = spaces.ToList<LODSet>();
			// }

			Arrange();
		}

		void DuplicateTeamSpaces()
		{
			var allSpaces = _trackSpaces.ToList();
			if( JediLounge ) allSpaces.Add( JediLounge );
			if( MissionControl ) allSpaces.Add( MissionControl );
			foreach( var track in allSpaces )
				if(  _teamSpaces[track].Count > 0) // we need at least one example team space
					while( _teamSpaces[track].Count < track.NumberOfTeams )
					{
						_teamSpaces[track].Add( 
							Instantiate( 
								_teamSpaces[track].FirstOrDefault().gameObject )
							.GetComponent<TeamSpaceDriver>() 
						);
					}
		}



		void Update()
		{
			if( IsLive ) Arrange();
		}


		void Arrange()
		{
			var pos = Vector3.up*MainHeight + Vector3.right*MainRadius;
			var rot = Quaternion.AngleAxis( 360f/_trackSpaces.Count, Vector3.up );
			
			// first place down the trackspaces in the inner ring
			foreach( var track in _trackSpaces )
			{
				track.transform.position = pos;
				track.Setup( transform, MainTetherRadius );
				pos = rot*pos;
			}


			// ... then the extra spaces
			if( JediLounge != null )
			{
				JediLounge.transform.position = Earth.JediSpaceLocation.position;
				JediLounge.Setup( Earth.transform );
			}
			if( MissionControl != null )
			{
				MissionControl.transform.position = Earth.MissionControlLocation.position;
				MissionControl.Setup( Earth.transform );
			}


			// then run through all the teamspaces and place them relative to the trackspaces
			// they're attached to
			var allSpaces = _trackSpaces.ToList();
			if( JediLounge != null ) allSpaces.Add( JediLounge );
			if( MissionControl != null ) allSpaces.Add( MissionControl );
			foreach( var track in allSpaces )
			{
				var tPos = 
					Quaternion.AngleAxis( TeamRotOffset, Vector3.up )
					*Quaternion.LookRotation( track.transform.position, Vector3.up )
						* Vector3.right
						* TeamsRadius 
					+ Vector3.up*TeamsHeight;
				var tRot = Quaternion.AngleAxis( 360f/_teamSpaces[track].Count, Vector3.up );
				// var tRot = Quaternion.LookRotation( track.transform.position, Vector3.up );
				foreach( var team in _teamSpaces[track] )
				{
					team.transform.position = track.transform.position + tPos +Vector3.up *Random.Range( 0, HeightVar );
					team.SetupTrackspaceConnection( track ); // ouch! Running this each frame! Only for testing purps!
					tPos = tRot * tPos;
					team.UpdateTether();
				}
			}
			DidArrange = true;
		}
	}
}