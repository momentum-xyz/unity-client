using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class NonUniformPosterRenderer : CustomContentRenderer, IPosterRenderer 
	{
		public enum ScaleDirection{X,Y}
		[Space]
		public ScaleDirection Axis = ScaleDirection.Y;
		public float InitialAspect = 1;

		public override bool Set( Texture2D texture ) => Set( texture, 1 );
		public bool Set( Texture2D texture, float ratio = 1 )
		{
			if( !base.Set(texture) ) return false;

			transform.localScale = 
				new Vector3(
					Axis==ScaleDirection.X?ratio/InitialAspect:1,
					Axis==ScaleDirection.Y?InitialAspect/ratio:1,
					1
				);

			return true;
		}


    }
}
