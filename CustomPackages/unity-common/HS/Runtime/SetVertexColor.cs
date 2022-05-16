using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVertexColor : MonoBehaviour
{
	public Color Color = Color.white;


	void Start()
	{
		var mesh = GetComponent<MeshFilter>().mesh;
		var colors = mesh.colors;
		if( colors==null || colors.Length == 0 )
			colors = new Color[mesh.vertexCount];
		for( int i = 0; i < colors.Length; i++ )
			colors[i] = Color;
		mesh.SetColors( colors );
	}

}
