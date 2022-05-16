using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	/// <summary> An optimised LOD element, in that it will only proc when the LOD
	/// level changes TO or FROM the defined setting.
	///	You can override the LOD setting here with a -1 to make sure it will
	/// proc with EVERY LOD change (see LODQuantum and LODMeshGroup for examples)
	/// </summary>
	public abstract class LODable : MonoBehaviour
	{
		/// <summary> The LOD level at which this object should change state. -1 means it
		/// will always change state when LOD levels change. </summary>
		public virtual int LOD => _lod;
		/// <summary> You can override this (with the 'new' keyword) and set it to -1
		/// to have a loddable be evaluated with EVERY change of LOD level. </summary>
		[SerializeField][Range(0,LODSet.MAXLOD)] int _lod;

		public abstract void SetLOD( int newLOD );
	}
}