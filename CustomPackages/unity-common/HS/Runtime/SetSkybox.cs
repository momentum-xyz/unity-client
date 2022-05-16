using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



namespace HS
{
	public class SetSkybox : MonoBehaviour
	{
		[SerializeField] Material _material;
		[SerializeField] AmbientMode AmbientMode = AmbientMode.Skybox;
		[SerializeField] float SkyboxIntensity = 1;
		[SerializeField] [ColorUsage( false, true )] Color AmbientColor = Color.grey;
		[SerializeField] [ColorUsage( false, true )] Color EquatorColor = Color.grey/2f;
		[SerializeField] [ColorUsage( false, true )] Color GroundColor = Color.grey/4f;

		public Cubemap sceneReflectionProbe;

		[ContextMenu( "Pickup From Scene Settings" )]
		void Pickup()
		{
			_material = RenderSettings.skybox;
			AmbientMode = RenderSettings.ambientMode;
			SkyboxIntensity = RenderSettings.ambientIntensity;
			AmbientColor = RenderSettings.ambientSkyColor;
			EquatorColor = RenderSettings.ambientEquatorColor;
			GroundColor = RenderSettings.ambientGroundColor;
		}


		void OnEnable()
		{
			// to force and ambient light update, we need to make sure we change the 
			// ambient mode to something else before setting it
			RenderSettings.ambientMode = 
				AmbientMode != UnityEngine.Rendering.AmbientMode.Custom
					? UnityEngine.Rendering.AmbientMode.Custom
					: UnityEngine.Rendering.AmbientMode.Flat;

			RenderSettings.skybox = _material;
			RenderSettings.ambientMode = AmbientMode;
			RenderSettings.ambientIntensity = SkyboxIntensity;
			RenderSettings.ambientSkyColor = AmbientColor;
			RenderSettings.ambientEquatorColor = EquatorColor;
			RenderSettings.ambientGroundColor = GroundColor;

	

			RenderSettings.ambientMode = AmbientMode;

			if (sceneReflectionProbe != null)
			{
				RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
				RenderSettings.customReflection = sceneReflectionProbe;
			}

			// (naam) I don't think we need this, as we're already setting the skybox to the new one? Not sure.
			// RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
			// RenderSettings.customReflection = (Cubemap)_material.GetTexture("_Tex");

			DynamicGI.UpdateEnvironment();

			SkyboxManager.CurrentSkyboxCubemap = (Cubemap)_material.GetTexture( "_Tex" );
		}
    }
}
