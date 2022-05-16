using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS 
{
	public class GridFlightTester : MonoBehaviour
	{
		[SerializeField] GridFlightController _manager;
		[SerializeField] KeyCode _skipBackKey = KeyCode.Comma;
		[SerializeField] KeyCode _skipForwKey = KeyCode.Period;
		[SerializeField] TMPro.TMP_Text _audioClockDisplay;


		void Update()
		{
			var t = _manager.AnthemPlayer.time-_manager.StartTime;
			_audioClockDisplay.text = $"{(int)t:000}:{(int)(t*60)%60:00}";
			if( Input.GetKeyDown( _skipBackKey ) )
				_manager.Skip( -10 );
			if( Input.GetKeyDown( _skipForwKey ) )
				_manager.Skip( 10 );
		}
	}
}