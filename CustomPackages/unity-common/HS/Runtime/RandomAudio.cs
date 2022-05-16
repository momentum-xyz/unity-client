using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class RandomAudio : MonoBehaviour
	{
		[SerializeField] Vector2 _minMaxPitch = new Vector2( 0.9f, 1.1f );
		AudioSource _audio;


		void Awake()
		{
			_audio = GetComponent<AudioSource>();
			_audio.playOnAwake = false;
		}

		void OnEnable()
		{
			_audio.pitch = Mathf.Lerp( _minMaxPitch.x, _minMaxPitch.y, Random.value );
			_audio.Play();
		}


	}
}