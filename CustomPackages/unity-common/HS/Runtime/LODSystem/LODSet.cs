using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using Nextensions;



namespace HS
{
	/// <summary> Should be on the root of the object that needs to respond to LOD procs. Contains
	/// a few handy authoring tools: <br/>
	/// 	- Set Current LOD (contextMenu)): sets the whole group to the given LOD, for testing in the editor<br/>
	/// 	- AutoSetup (contextMenu): looks for @LOD00 @LOD01 ... tags in object names and adds appropriate 
	/// LODQuantums to them.
	/// </summary>
	public class LODSet : MonoBehaviour
	{
		public static HashSet<LODSet> Members = new HashSet<LODSet>();



		/// <summary> Only meant as an editor-accessible test parameter. Please drive this
		/// behaviour via SetLod( newLod )</summary>
		[SerializeField][Range(0,MAXLOD)] int _lod = 0;
		[SerializeField] bool _updateInEditor;


		public const int MAXLOD = 3;

		int _lastLOD; 


		Dictionary<int,HashSet<LODable>> _internalMembers = new Dictionary<int, HashSet<LODable>>();


		void OnValidate()
		{
			if( _updateInEditor && Application.isPlaying == false && Application.isEditor )
				SetToCurrentLODEditor();
		}


		/// <summary> Only meant to test-set the current LOD to the appropriate objects in Editor. Please use
		/// SetLOD( newLod ) to drive this behaviour via script.</summary>
		[ContextMenu( "SetCurrentLOD" )]
		void SetToCurrentLODEditor()
		{
			_internalMembers.Clear();
			foreach( var elm in GetComponentsInChildren<LODable>( true ) )
				Register( elm );
			SetLOD( _lod, true );
		}


		/// <summary> Searches for child objects tagged with @LOD00 @LOD01 ... and gives them an approprariate
		/// LODQuantum. Meant for authoring ease of setup. </summary>
		[ContextMenu( "AutoSetup" )]
		void Setup()
		{
			foreach( var op in this.FindByTaggedNameAll( "@LOD" ) )
			{
				// Debug.Log( op.gameObject.name.ToUpper() );
				var lod = op.CreateOrAddComponent<LODQuantum>();
				var array = new bool[MAXLOD+1];
				var matches = Regex.Matches( op.gameObject.name.ToUpper(), "\\@LOD(\\d+)" );
				foreach( Match match in matches )
					if( match.Success )
						array[System.Int32.Parse(match.Groups[1].Value)] = true;
				lod.Levels = array;
			}
		}


		/// <summary> Called by LOD elements to register themselves with this LOD Setter. </summary>
		public void Register( LODable elm )
		{
			if( !(_internalMembers.ContainsKey( elm.LOD ) ) ) _internalMembers.Add( elm.LOD, new HashSet<LODable>() );
			_internalMembers[elm.LOD].Add( elm );
		}


		/// <summary> Called by LOD elements to Deregister themselves when destroyed. </summary>
		public void Deregister( LODable elm )
		{
			if( _internalMembers.ContainsKey( elm.LOD ) ) _internalMembers[elm.LOD].Remove( elm );
		}


		/// <summary> Changes only those LOD elements that should change. Or (with Enforce on) resets
		/// everything to the newly given LOD. </summary>
		public void SetLOD( int newLOD, bool enforceUpdate = false )
		{
			if( newLOD == _lastLOD && !enforceUpdate ) return;

			// we will only run by objects that shoudl be changed
			int minLod = 0;
			int maxLod = MAXLOD;
			if( !enforceUpdate )
			{
				minLod = _lastLOD;
				maxLod = newLOD;
				if( maxLod<minLod ) (maxLod,minLod) = (minLod,maxLod); // WOAH does this work!
			}
			// string result = $"moving from LOD{_lod} to LOD{newLOD} su running by these: ";
			for( int i = minLod; i <= maxLod; i++ )
			{
				// result += $"{i}  ";
				if( _internalMembers.ContainsKey( i ) )
					foreach( var elm in _internalMembers[i] ) 
						elm.SetLOD( newLOD );
			}
			if( _internalMembers.ContainsKey( -1 ) )
				foreach( var elm in _internalMembers[-1] ) 
					elm.SetLOD( newLOD );
			// Debug.Log( result );

			_lod = newLOD;
			_lastLOD = newLOD;
		}


		void Awake()
		{
			_internalMembers.Clear();
			foreach( var elm in GetComponentsInChildren<LODable>( true ) )
				Register( elm );
			SetLOD( _lod, true );
		}


		void OnEnable() => Members.Add(this);
		void OnDisable() => Members.Remove(this);
	}
}