using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class NameRibbon : MonoBehaviour
	{

		public Texture2D Texture;
		public int IndexOnTexture;
		public int LinesOnTexture = 15;


		UserPlatformDriver _driver;


		[ContextMenu( "Test On Shared Material" )]
		void TestWithSharedMaterial() => Set( Texture, IndexOnTexture, LinesOnTexture, true );


		public void Set( Texture2D texture, int line, int lineCount, bool useSharedMaterial = false )
		{
			Texture = texture;
			IndexOnTexture = line;
			LinesOnTexture = lineCount;
			var mat = 
				useSharedMaterial   
					? GetComponent<Renderer>().sharedMaterial
					: GetComponent<Renderer>().material;
			mat.mainTexture = Texture;
			mat.mainTextureOffset = new Vector2(
				0,
				-1-(float)(line+1)/(float)LinesOnTexture
			);
			mat.mainTextureScale = new Vector2(
				1,
				1f/LinesOnTexture
			);
		}


		void Awake()
		{
			_driver = GetComponentInParent<UserPlatformDriver>();
			_driver?.Ribbons.Add(this);
			if( Texture != null ) Set( Texture, IndexOnTexture, LinesOnTexture ) ;
		}


		void OnDestroy()
		{
			_driver?.Ribbons.Remove(this);
		}


	}
}