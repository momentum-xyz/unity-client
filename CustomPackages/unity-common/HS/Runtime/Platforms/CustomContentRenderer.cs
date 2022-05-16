using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class CustomContentRenderer : MonoBehaviour
	{
		[Header("Elements")]
		public bool Self = true;
		public List<Renderer> Renderers = new List<Renderer>();

		public virtual bool Set( Texture2D texture )
		{
			if( Application.isPlaying == false )
			{
				Debug.LogError( "Only allowed in Play mode" );
				return false;
			}
			foreach( var r in Renderers ) r.material.SetTexture( "_BaseMap", texture );
			foreach( var r in Renderers ) r.material.SetTexture( "_MainTex", texture );
			return true;
		}


		protected virtual void Awake()
		{
			if( Self )
			{
				var r = GetComponent<Renderer>();
				if( Renderers == null ) Renderers = new List<Renderer>();
				if( r!=null && Renderers.Contains(r)==false )
					Renderers.Add(r);
			}
		}

	}
}
