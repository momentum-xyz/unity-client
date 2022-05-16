using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class LODTester : MonoBehaviour
	{
		public bool FindAutomatically;
		// public List<LODSet> Setters;
		public List<float> Distances = new List<float>{40, 60, 350};

		Camera _cam;


		// void Start()
		// {
		// 	if( FindAutomatically )
		// 	{
		// 		foreach( var s in FindObjectsOfType<UserPlatformDriver>(true) )
		// 			foreach( var l in s.LODSets )
		// 				Setters.Add(l);
		// 	}
		// }


		void Update()
		{
			if( !_cam ) _cam = Camera.main;


			foreach( var op in LODSet.Members )
			{
				var dist = (op.transform.position - _cam.transform.position ).sqrMagnitude;
				int LOD = 0;
				while( LOD < Distances.Count && dist >= Distances[LOD]*Distances[LOD] ) LOD++;
				op.SetLOD( LOD );
			}
		}
	}
}