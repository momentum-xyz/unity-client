using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class DistributeFogSettings : MonoBehaviour
	{
		public bool Fog;
		public Color Color = Color.gray;
		public FogMode FogMode = FogMode.ExponentialSquared;
		public float Density = 0.004f;
		public Vector2 StartEnd = new Vector2( 5, 200 );

		[ContextMenu( "Pickup Settings From Lighting" )]
		void Pickup()
		{
			Fog = RenderSettings.fog;
			Color = RenderSettings.fogColor;
			FogMode = RenderSettings.fogMode;
			Density = RenderSettings.fogDensity;
			StartEnd = new Vector2(
				RenderSettings.fogStartDistance,
				RenderSettings.fogEndDistance
			);
		}

		[ContextMenu( "Set Scene Fog")]
		void OnEnable()
		{
			RenderSettings.fog = Fog;
			RenderSettings.fogColor = Color;
			RenderSettings.fogMode = FogMode;
			RenderSettings.fogDensity = Density;
			RenderSettings.fogStartDistance = StartEnd.x;
			RenderSettings.fogEndDistance = StartEnd.y;
		}

	}
}