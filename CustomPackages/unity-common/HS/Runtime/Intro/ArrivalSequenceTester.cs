using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS 
{
	public class ArrivalSequenceTester : MonoBehaviour
	{
		[SerializeField] ArrivalSequenceManager _manager;
		[SerializeField] KeyCode _nextPhaseButton = KeyCode.Tab;
		[SerializeField] KeyCode _skipBackKey = KeyCode.Comma;
		[SerializeField] KeyCode _skipForwKey = KeyCode.Period;
		[SerializeField] AudioSource _audio;
		[SerializeField] TMPro.TMP_Text _audioClockDisplay;


		void Update()
		{
			var t = _audio.time;
			_audioClockDisplay.text = $"{(int)t:000}:{(int)(t*60)%60:00}";
			if( Input.GetKeyDown( _nextPhaseButton ) )
				_manager.NextPhase();
			if( Input.GetKeyDown( _skipBackKey ) )
				_manager.Skip( -10 );
			if( Input.GetKeyDown( _skipForwKey ) )
				_manager.Skip( 10 );
		}
	}
}