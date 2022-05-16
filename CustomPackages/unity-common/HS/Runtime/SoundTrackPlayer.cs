using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;



namespace HS
{
	public class SoundTrackPlayer : MonoBehaviour
	{
		[SerializeField] KeyCode _prevKey = KeyCode.Comma;
		[SerializeField] KeyCode _nextKey = KeyCode.Period;
		[SerializeField] KeyCode _pauseKey = KeyCode.Slash;
		[SerializeField] List<AudioClip> _songs;
		[SerializeField] GameObject _panel;

		AudioSource _audio;
		List<AudioClip> _playList = new List<AudioClip>();
		int _idx;
		bool _paused;


		public void PreviousSong()
		{
			_idx = (_idx+_playList.Count-2)%_playList.Count;
			_audio.Stop();
		}

		public void NextSong()
		{
			_audio.Stop();
		}

		public void PausePlayer()
		{
			_paused = !_paused;
			if( _paused ) _audio.Pause();
			else _audio.UnPause();
			_panel.transform.Find( "[SONGTITLE]" ).GetComponent<TMPro.TMP_Text>().text = 
				_paused
					? "(pause)" 
					: Regex.Replace( _audio.clip.name, ".* - .* - ", "" );;
			_panel.GetComponent<Animator>().Rebind();
		}



		void Awake()
		{
			_audio = GetComponent<AudioSource>();
		}


		void OnEnable()
		{
			StartCoroutine( PlayAlbum() );
		}


		IEnumerator PlayAlbum()
		{
			Shuffle();
			_audio.Stop();
			_audio.loop = false;
			_audio.clip = null;

			_idx = 0;
			while( true )
			{
				_audio.clip = _playList[_idx];
				_audio.Play();
				_panel.transform.Find( "[SONGTITLE]" ).GetComponent<TMPro.TMP_Text>().text = Regex.Replace( _audio.clip.name, ".* - .* - ", "" );
				_panel.GetComponent<Animator>().Rebind();
				yield return new WaitUntil( ()=> _audio.isPlaying == false && !_paused );
				_idx = (_idx+1)%_playList.Count;
			}
		}

		void Update()
		{
			if( Input.GetKeyDown( _prevKey ) )
				PreviousSong();
			if( Input.GetKeyDown( _nextKey ) )
				NextSong();
			if( Input.GetKeyDown( _pauseKey ) )
				PausePlayer();
		}


		void Shuffle()
		{
			var bag = new List<AudioClip>( _songs );
			_playList.Clear();
			for( int i = 0; i < _songs.Count; i++ )
			{
				if( bag.Count == 1 )
				{
					_playList.Add( bag[0] );
					bag.RemoveAt( 0 );
					break;
				}
				else
				{
					var idx = Random.Range( 0, bag.Count );
					_playList.Add( bag[idx] );
					bag.RemoveAt( idx );
				}
			}
		}
	}
}