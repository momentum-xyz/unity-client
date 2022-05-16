using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class MotionSound : MonoBehaviour
	{
		[SerializeField] float _referenceSpeed = 10;
		[SerializeField] float _volume = 1f;
		[SerializeField] AnimationCurve _volumeEnvelope;
		[SerializeField] Vector2 _minMaxPitch = new Vector2( 0.7f, 1.2f );
		[SerializeField] AnimationCurve _pitchEnvelope;

		AudioSource _audio;
		ThirdPersonController _driver;

		float _speed;

		void Awake()
		{
			_audio = GetComponent<AudioSource>();
			_driver = GetComponentInParent<ThirdPersonController>();

			_audio.volume = 0;
			_audio.pitch = 1;
		}


		void Update()
		{
			_speed = Mathf.Lerp( _speed, _driver.AbsSpeed, Time.unscaledDeltaTime * 4 ); // MAGIC
			var newVolume = Mathf.Clamp01( _volumeEnvelope.Evaluate( _speed/_referenceSpeed ) * _volume );
			if( float.IsNaN( newVolume ) == false )
				_audio.volume = newVolume;
			var newPitch = Mathf.Clamp( Mathf.Lerp( _minMaxPitch.x, _minMaxPitch.y, _pitchEnvelope.Evaluate( _speed/_referenceSpeed ) ), 0.1f, 3 );
			if( float.IsNaN( newPitch ) == false )
				_audio.pitch = newPitch;
		}
	}
}