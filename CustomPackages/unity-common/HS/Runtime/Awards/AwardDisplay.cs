using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS
{
	public class AwardDisplay : MonoBehaviour
	{
		[SerializeField] Renderer _renderer;


		public void Setup( Awards.AwardDefinition def )
		{
			var mat = _renderer.material;
			mat.SetTexture( "_BaseMap", def.RGBA );	
			mat.SetTexture( "_NormalMap", def.Normal );	
		}
	}
}