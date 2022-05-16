using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;



namespace HS
{
	public class AnimatedClones : MonoBehaviour
	{
		public int Clones = 5;

		HashSet<GameObject> _clones = new HashSet<GameObject>();


		void OnEnable()
		{
			foreach( var c in _clones ) Destroy( c );
			_clones.Clear();
			var source = transform.GetChild(0).gameObject;
			source.SetActive(false);

			for(int i = 0; i < Clones; i++ )
			{
				var newOp = source.SpawnInside(transform);
				var anm = newOp.GetComponent<Animator>();
				anm.SetFloat( "Offset", (float)i/(float)(Clones-1) );
				_clones.Add(newOp);
			}
		}
	}
}
