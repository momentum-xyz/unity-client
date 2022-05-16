using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class LODElement : LODable
	{
		// public int LOD => _lod;
		// [SerializeField][Range(0,LODSets.MAXLOD)]int _lod;


		public enum Method{ Equal, Less, Greater }
		public Method ActiveWhen = Method.Less;

		LODSet _set;


		void Awake()
		{
			_set = GetComponentInParent<LODSet>();
			_set?.Register( this ); // decided to also look for ILODables in LODSets.Awake, so this should strictly not be necessary
		}


		void OnDestroy()
		{ 
			_set?.Deregister( this );
		}


		public override void SetLOD( int newLOD )
		{
			switch( ActiveWhen )
			{
				case Method.Equal:		gameObject.SetActive( newLOD == LOD ); 	break;
				case Method.Less:		gameObject.SetActive( newLOD < LOD ); 	break;
				case Method.Greater:	gameObject.SetActive( newLOD > LOD ); 	break;
			}
		}
	}
}