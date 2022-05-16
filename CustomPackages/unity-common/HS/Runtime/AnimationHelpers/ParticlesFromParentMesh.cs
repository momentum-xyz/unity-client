using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	/// <summary> Effect authoring helper. Picks up the mesh it finds in the parent 
	/// to use as a particle mesh. Automatically disables parent renderer. </summary>
	public class ParticlesFromParentMesh : MonoBehaviour
	{	
			[Header( "Make sure the mesh is read/write enabled" )]
			[SerializeField] bool _doPickupNow;


			void OnValidate()
			{
				if( _doPickupNow ) Pickup();
				_doPickupNow = false;
			}



			[ContextMenu( "Pickup" )]
			void Pickup()
			{
				var prt = GetComponentInChildren<ParticleSystem>(true);
				if( !prt ) return;
				var pRnd = prt.GetComponent<ParticleSystemRenderer>();
				if( !pRnd ) return;
				var mFil = GetComponentInParent<MeshFilter>();
				if( !mFil ) return;
				var mRnd = mFil.GetComponent<MeshRenderer>();
				if( mRnd ) mRnd.enabled = false;
				pRnd.renderMode = ParticleSystemRenderMode.Mesh;
				pRnd.mesh = mFil.sharedMesh;
			}


			void Awake()
			{
				Pickup();
			}
	}
}