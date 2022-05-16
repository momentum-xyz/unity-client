using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace HS
{
	public class ArrivalSequenceManager : MonoBehaviour
	{
		[SerializeField] AudioSource _launchAnthem;
		[SerializeField] GameObject _soundTrack;
		[SerializeField] GameObject _fakeAvasForShortVersion;
		[SerializeField] Button _skipIntroButton;
		[SerializeField] GameObject _skipConfirmWindow;
		[SerializeField] Button _skipConfirmButton;
		[SerializeField] Button _skipCancelButton;
		[SerializeField] KeyCode _skipIntroKey = KeyCode.X;
		[SerializeField] KeyCode _cancelKey = KeyCode.Z;
		[SerializeField] string _nextAnimationScene;
		[SerializeField] string _skipToScene;

		[Header( "Short version stuff" )]
		public bool ShortVersion;
		// [SerializeField] float _shortVersionMusicOffset;


		bool _skipIsActive => ShortVersion;


		bool _isPlayingAnthemSequence;
		// float _preRoll; // only needed for short version (counts the time the anthem has already been playing before starting the Launch animation, so we can align the two)
		// bool _runningPreroll;
		Animator _anm;



		/// <summary> Triggers the LAUNCH event animation and music. If you
		/// also feed it a time in seconds, it will try to catch up. This is so 
		/// people logging in late will catch it happen semi-simultaneously
		/// </summary>
		public void TriggerLaunch( float secondsIntoLaunch = 0 )
		{
			if( ShortVersion ) return;
			_anm.SetTrigger( "LaunchEvent" );
			if( secondsIntoLaunch>0 )
				StartCoroutine( Catchup( secondsIntoLaunch ) );
		}



		/// <summary> Call by Animation event only please! </summary>
		public void ANMTriggerNextSceneLoad()
		{
			SceneManager.LoadScene( _nextAnimationScene );
		}

		/// <summary> Call by Animation event or internally only please! </summary>
		public void ANMTriggerAnthemShortVersion()
		{
			if( ShortVersion )
			{
				// _runningPreroll = true;
				StartCoroutine( InitAnthem() );
			}
		}
		/// <summary> Call by Animation event or internally only please! </summary>
		public void ANMTriggerAnthem()
		{
			if( ShortVersion ) // we're already playing, but we should align with the newly started launch animation.
			{
				AlignAnimationWithSound();
				// StartCoroutine( AlignAnthem() );
			}
			else
				StartCoroutine( InitAnthem() );
		}

		IEnumerator InitAnthem()
		{
			if( !_launchAnthem.isPlaying )
				_launchAnthem.Play();
			DontDestroyOnLoad( _launchAnthem.gameObject );
			_isPlayingAnthemSequence = true;
			yield return null; // just so the autodestruct doesn't immediately trigger
			_launchAnthem.gameObject.AddComponent<AutoDestructEffect>();
		}


		// IEnumerator AlignAnthem()
		// {
		// 	yield return new WaitForSeconds(1);
		// 	// _runningPreroll = false;
		// 	// Debug.Log( _preRoll );
		// }

		/// <summary> PROTOTYPING FUNCTION </summary>
		public void NextPhase()
		{
			if( _isPlayingAnthemSequence )
			{
				_isPlayingAnthemSequence = false;
				_launchAnthem.Stop();
				_launchAnthem.time = 0;
				Destroy( _launchAnthem.GetComponent<AutoDestructEffect>() );
			}
			_anm.SetTrigger( "Next" );
		}

		
		/// <summary> PROTOTYPING FUNCTION </summary>
		public void Skip( float seconds )
		{
			if( !_isPlayingAnthemSequence ) return;
			var newTime = _launchAnthem.time +seconds;
			newTime = Mathf.Clamp( newTime, 0, _launchAnthem.clip.length );
			_launchAnthem.time = newTime;
			AlignAnimationWithSound();
		}


		IEnumerator Catchup( float seconds )
		{
			// give the animation a few frames to trigger ne necessities
			yield return null;
			yield return null;

			_launchAnthem.time = seconds;
			AlignAnimationWithSound();

			yield break;
		}



		void Awake()
		{
			_soundTrack.SetActive( false );
			_anm = GetComponent<Animator>();
			_skipConfirmWindow.SetActive( false );
		}

		void OnEnable()
		{
			// if( _skipIsActive ) 
			_skipIntroButton.onClick.AddListener( SkipRequest );
			_skipConfirmButton.onClick.AddListener( SkipConfirm );
			_skipCancelButton.onClick.AddListener( SkipCancel );
		}

		void OnDisable()
		{
			_skipIntroButton.onClick.RemoveListener( SkipRequest );
			_skipConfirmButton.onClick.RemoveListener( SkipConfirm );
			_skipCancelButton.onClick.RemoveListener( SkipCancel );
		}

		IEnumerator Start()
		{
			_skipIntroButton.gameObject.SetActive( _skipIsActive );
			yield return new WaitForSeconds( 0.1f ); // give a chance for the wrapper to set ShortVersion
			_anm.SetBool( "Short", ShortVersion );
			_skipIntroButton.gameObject.SetActive( _skipIsActive );
			_fakeAvasForShortVersion.SetActive( ShortVersion );
			_soundTrack.SetActive( !ShortVersion );
		}

		void Update()
		{
			// if( _runningPreroll ) _preRoll += Time.unscaledDeltaTime;
			if( Input.GetKeyDown( _skipIntroKey ) )
				if( _skipIntroButton.gameObject.activeInHierarchy ) SkipRequest();
				else if( _skipConfirmWindow.activeInHierarchy ) SkipConfirm();
			if( Input.GetKeyDown( _cancelKey ) && _skipConfirmWindow.activeInHierarchy ) SkipCancel();
		}


		void AlignAnimationWithSound()
		{
			if( _anm && _isPlayingAnthemSequence )
			{
				_anm.CrossFadeInFixedTime( "LaunchSequence", 0, 0, _launchAnthem.time );//-(_shortVersion?_preRoll:0) );
			}
		}

		void SkipRequest()
		{
			if( !_skipIsActive ) return;
			_skipConfirmWindow.SetActive( true );
			_skipIntroButton.gameObject.SetActive( false );
		}

		void SkipCancel()
		{
			_skipConfirmWindow.SetActive( false );
			_skipIntroButton.gameObject.SetActive( true );
		}

		void SkipConfirm()
		{
			Destroy( _launchAnthem.gameObject );
			SceneManager.LoadScene( _skipToScene );
		}
	}
}