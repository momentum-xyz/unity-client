using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class AttractChild : MonoBehaviour
	{
		[Range(0,60)] public float Force = 0;


		void Update()
		{
			if( Force <= 0 ) return;
			var child = transform.GetChild( 0 );
			if( !child ) return;
			if( child.localPosition.sqrMagnitude < 0.05f*0.05f ) return;
			child.localPosition = Vector3.Lerp( child.localPosition, Vector3.zero, Time.deltaTime*Force );
		}
	}
}