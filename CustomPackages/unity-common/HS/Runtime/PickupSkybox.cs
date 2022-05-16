using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class PickupSkybox : MonoBehaviour
	{
		ReflectionProbe _probe;

		private bool pickupAtSpawn = true;

		[ContextMenu("Pickup From Scene")]
		void PickupFromScene()
		{
			GetComponent<ReflectionProbe>().mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
			GetComponent<ReflectionProbe>().customBakedTexture = RenderSettings.skybox.mainTexture;
		}

		void OnEnable()
		{
			SkyboxManager.OnChange -= Pickup;
			SkyboxManager.OnChange += Pickup;

			// Pickup the Skybox at spawn, but just do it only then
			if (pickupAtSpawn)
			{
				pickupAtSpawn = false;
				Pickup();
			}
		}
		void OnDisable()
		{
			SkyboxManager.OnChange -= Pickup;
		}

		void Pickup()
		{
			if (!_probe) _probe = GetComponent<ReflectionProbe>();
			if (_probe)
			{
				_probe.mode = UnityEngine.Rendering.ReflectionProbeMode.Custom;
				_probe.customBakedTexture = SkyboxManager.CurrentSkyboxCubemap;
			}
		}
	}
}
