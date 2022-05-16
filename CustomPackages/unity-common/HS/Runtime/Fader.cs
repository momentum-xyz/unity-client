using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	[ExecuteAlways]
	public class Fader : MonoBehaviour
	{
		public Texture2D Texture;
		public Color Color = Color.white;
		[Range(0,1)] public float Alpha = 0;



		void OnGUI()
		{
			// var c = GUI.color;
			var c = Color;
			c.a = Alpha;
			GUI.color = c;
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), Texture );
		}
	}
}