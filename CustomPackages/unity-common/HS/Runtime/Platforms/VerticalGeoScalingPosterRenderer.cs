using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class VerticalGeoScalingPosterRenderer : CustomContentRenderer, IPosterRenderer
	{
	
		
		[Header( "The Top bit just moves up and down" )]
		public Transform Top;
		[Header( "The Center bit accomodates for aspect ratio" )]
		public Transform Center;
		[Header( "The Bottom bit just moves up and down" )]
		public Transform Bottom;
		public float Size = 1;
		public float InitialAspect = 1;
		[SerializeField] Vector2 _minMaxScale = new Vector2( 0.35f, 1.5f );

		public override bool Set( Texture2D texture ) => Set( texture, 1 );
		public bool Set( Texture2D texture, float ratio = 1 )
		{
			if( !base.Set(texture) ) return false;

			var f = 1/ratio -1;
			if( Top ) Top.localPosition = Vector3.up * f *Size;
			if( Bottom ) Bottom.localPosition = -Vector3.up * f *Size;
			if( Center ) Center.localScale = new Vector3(1,1/ratio,1);

			transform.localScale = Vector3.one * Mathf.Clamp( Mathf.Sqrt(ratio), _minMaxScale.x, _minMaxScale.y );

			return true;
		}



		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			Gizmos.matrix = transform.localToWorldMatrix;

			Gizmos.DrawWireCube( Vector3.forward * -2.5f, new Vector3(Size*2,Size*2,0.01f) );
		}

	}
}
