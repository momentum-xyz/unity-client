using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;



namespace HS
{
	public class PosterRenderer : CustomContentRenderer, IPosterRenderer
	{
		[SerializeField] bool _dontScale;
		[SerializeField] Vector2 _baseRatio = new Vector2(3,4);
		[SerializeField] Vector2 _minMaxScale = new Vector2( 0.35f, 3f );

		Transform _align;
		Vector3 _basePos;
		Vector3 _baseScale;


		[ContextMenu( "Pick Ratio from name" )]
		public void PickupRatio()
		{
			var (ratioA,ratioB) = name.PickupRatioFactors((int)_baseRatio.x,(int)_baseRatio.y);
			_baseRatio = new Vector2( ratioA, ratioB );
		}



		public override bool Set( Texture2D texture ) => Set( texture, 3f/4f );
		public virtual bool Set( Texture2D texture, float ratio = 3f/4f )
		{
			if( Application.isPlaying == false )
			{
				Debug.LogError( "Only allowed in Play mode" );
				return false;
			}
			foreach( var r in Renderers ) r.material.SetTexture( "_BaseMap", texture );
			foreach (var r in Renderers) r.material.SetTexture("_MainTex", texture);

			if ( _dontScale ) return true;
			
			// for now we keep Y size and adjust X size if necessary
			var f = ratio/(_baseRatio.x/_baseRatio.y);
			var fClamped = Mathf.Clamp(f,_minMaxScale.x,_minMaxScale.y);
			var fInverse = fClamped/f;
			transform.localScale = 
				new Vector3(
					f *_baseScale.x,
					_baseScale.y,
					_baseScale.z
				);
			transform.localScale = 
				transform.localScale *fInverse;
			
			if( _align )
			{
				transform.position += 
					transform.parent.TransformPoint(_basePos)
					-_align.position;
			}

			return true;
		}


		protected override void Awake()
		{
			base.Awake();
			_baseScale = transform.localScale;
			_align = this.FindByTaggedName( "@align" );
			_basePos = 
				_align
					? transform.parent.InverseTransformPoint( _align.position )
					: transform.localPosition;
		}
	}
}
