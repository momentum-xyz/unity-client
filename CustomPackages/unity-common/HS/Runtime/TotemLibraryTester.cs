using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Nextensions;



namespace HS
{
	public class TotemLibraryTester : MonoBehaviour
	{
		public List<GameObject> Totems;
		public KeyCode TotemKey = KeyCode.T;

		void Update()
		{
			if( Input.GetKeyDown( TotemKey ) )
			{
				var loc = FindObjectsOfType<TrackSpaceDriver>().ToList().PickOne().FindByTaggedName<Transform>("@Totem");
				foreach( Transform t in loc ) Destroy( t.gameObject );
				Totems.PickOne().SpawnInside( loc );
			}
		}
	}
}