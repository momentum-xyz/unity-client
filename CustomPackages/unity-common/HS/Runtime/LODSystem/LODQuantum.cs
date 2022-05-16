using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	/// <summary> A simple checkbox per LOD level. This LODable procs with -every- LOD change.
	/// Mainly meant for automation: this one gets added by LODSet's automatic setup script.
	/// </summary>
	public class LODQuantum : LODable
	{
		public override int LOD => -1;
		[Header( "(LOD setting above is ignored, this object always changes)" )]
		[SerializeField] bool Understood;

		public bool[] Levels;


		LODSet _set;


		void Awake()
		{
			_set = GetComponentInParent<LODSet>();
			_set?.Register( this );
		}


		void OnDestroy()
		{ 
			_set?.Deregister( this );
		}


		public override void SetLOD( int newLOD )
		{
			// Debug.Log( $"Quantum setting LOD to {newLOD}" );
			if( newLOD > Levels.Length-1 )
				gameObject.SetActive( false );
			else
				gameObject.SetActive( Levels[newLOD] );
		}
	}
}