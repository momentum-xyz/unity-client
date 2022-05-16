using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HS
{
	public class HighFiveEvent : MonoBehaviour
	{
		public HighFiveSettings Settings;

		private AvatarDriver reciever;

		public AvatarDriver Reciever
		{
			get => reciever;
			set => reciever = value;
		}

		private AvatarDriver sender;

		public AvatarDriver Sender
		{
			get => sender;
			set => sender = value;
		}

		/// <summary> Creates a High-five graphical effect between Sender and Reciever. It will be 
		/// situated at the reciever's end. </summary>
		public static void Create(Vector3 SenderLocation, Vector3 RecieverLocation, HighFiveSettings settings = null)
		{
			DefaultsManager.TriggerDefaulLoad();
			settings = settings ?? HighFiveSettings.Default;

			var effect = HS.Pool.Instance.GetSpawnFromPrefab( settings.HighFiveEffectPrefab );
			effect?.transform.Place( RecieverLocation );
			effect?.transform.OrientTowards( SenderLocation );

			var arc = HS.Pool.Instance.GetSpawnFromPrefab( settings.ArcPrefab )?.GetComponent<ConnectionArcDriver>();
			arc?.Setup( SenderLocation, RecieverLocation );
		}
	}
}

#region Old HighFive event class

//old highFive Event Class
/*
 public class HighFiveEvent : MonoBehaviour
	{

		private AvatarDriver reciever;
		private AvatarDriver sender;

		public static HighFiveEvent Create(AvatarDriver reciever, AvatarDriver sender, HighFiveSettings settings)
		{
			settings.Reciever = reciever;
			settings.Sender = sender;

			return Create(settings);
		}

		public static HighFiveEvent Create(HighFiveSettings settings)
		{
			var result = new GameObject("HighFiveEvent").AddComponent<HighFiveEvent>();
			result.reciever = settings.Reciever;
			result.sender = settings.Sender;
			result.Location = settings.Reciever.transform.position;
			return result;
		}

		public void Awake()
		{
			//play ParticleSystem
			for (int i = 0; i < settings.Particles.Length; i++)
			{
				settings.Particles[i].transform.position = Location;
				if(settings.Particles[i].transform.childCount > 0)
					settings.Particles[i].Play(true);
				else
					settings.Particles[i].Play();
			}
			
		}

		private IEnumerator FinishingEvents()
		{
			while (true)
			{
				yield return new WaitForSeconds(.5f);
				bool breakWhileLoop = false;

				if (settings.Particles.Length > 0)
					for (int i = 0; i < settings.Particles.Length; i++)
					{
						if (settings.Particles[i].isPlaying)
							breakWhileLoop = false;
						else
						{
							breakWhileLoop = true;
							break;
						}
					}

				if (breakWhileLoop && settings.AudioSources.Length > 0)
					for (int i = 0; i < settings.AudioSources.Length; i++)
					{
						if (settings.AudioSources[i].isPlaying)
							breakWhileLoop = false;
						else
						{
							breakWhileLoop = true;
							break;
						}
					}

				if (breakWhileLoop && settings.animations.Length > 0)
					for (int i = 0; i < settings.animations.Length; i++)
					{
						if (settings.animations[i].isPlaying)
							breakWhileLoop = false;
						else
						{
							breakWhileLoop = true;
							break;
						}
					}

				if (breakWhileLoop)
					break;
			}
		}
	}
}
 * 
 */

#endregion