using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class TrackNameTexture : MonoBehaviour
	{
		public int TrackNumber;
		public int NumberOfLines = 15;

		[ContextMenu( "Test Setup" )]
		void TestWithSharedMaterial() => SetLine( TrackNumber, true );

		void Start()
		{
			//SetLine( TrackNumber );
		}


		void SetLine( int line, bool useSharedMaterial = false )
		{
			var mat = 
				useSharedMaterial   
					? GetComponent<Renderer>().sharedMaterial
					: GetComponent<Renderer>().material;
			mat.mainTextureOffset = new Vector2(
				0,
				-1-(float)(line+1)/(float)NumberOfLines
			);
		}

	}
}