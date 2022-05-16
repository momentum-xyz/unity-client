using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	/// <summary> Uses its own Meshfilter as LOD0, and its children as LOD1, LOD2 etc
	/// </summary>
	public class LODMeshGroup : LODable
	{
		public override int LOD => -1;
		[Header( "Changes own and child renderers according to LOD level, always." )]
		[SerializeField] bool Understood;

		[Space]
		[SerializeField] bool _keepLastChild;


		List<MeshRenderer> _meshes = new List<MeshRenderer>();
		bool _readInMeshes = false;


		[ContextMenu( "Setup child meshes" )]
		public void CreateLODChildren()
		{
			var ops = new List<Transform>();
			for( int i = 1; i <= LODSet.MAXLOD; i++ )
			{
				var op = Instantiate( gameObject );
				var destroy = new List<Transform>();
				foreach( Transform t in op.transform ) destroy.Add( t );
				foreach( var t in destroy ) DestroyImmediate( t.gameObject );
				op.name += $"_LOD{i}";
				DestroyImmediate( op.GetComponent<LODMeshGroup>() );
				ops.Add( op.transform );
			}
			foreach( var op in ops )
			{
				op.transform.SetParent( transform );
				op.transform.Zero();
			}
		}



		void Awake()
		{
			ReadMeshes();
		}


		void ReadMeshes()
		{
			_meshes.Clear();
			_meshes.Add( GetComponent<MeshRenderer>() );
			for( int i = 0; i < transform.childCount; i++ )
				_meshes.Add( transform.GetChild(i).GetComponent<MeshRenderer>() );
			_readInMeshes = true;
		}


		void OnDestroy()
		{ 
			GetComponentInParent<LODSet>()?.Deregister( this );
		}


		public override void SetLOD( int newLOD )
		{
			if( !_readInMeshes ) ReadMeshes();
			if( _keepLastChild ) newLOD = Mathf.Min( newLOD, _meshes.Count-1 );
			for( int i = 0; i < _meshes.Count; i++ )
				if( _meshes[i]!=null )
				{
					_meshes[i].gameObject.SetActive(true); // LOD objects are often hidden during authoring
					_meshes[i].enabled = i==newLOD;
				}
		}
	}
}
