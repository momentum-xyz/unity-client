using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



namespace HS
{
	public class GridFlightController : MonoBehaviour
	{
		[SerializeField] public AudioSource AnthemPlayer;
		[SerializeField] public float StartTime;
		[SerializeField] Button _skipButton;
		[SerializeField] GameObject _skipConfirmWindow;
		[SerializeField] Button _skipConfirmButton;
		[SerializeField] Button _skipCancelButton;
		[SerializeField] KeyCode _skipKey = KeyCode.X;
		[SerializeField] KeyCode _cancelKey = KeyCode.Z;
		[SerializeField] string _nextScene;
		[SerializeField] GameObject _nextSceneFaderPrefab;

		bool _isPlayingAnthemSequence = true;

		LaunchAnthemLocator _incomingLaunchAnthem;
		Animator _anm;
		Animation _animation;


		/// <summary> Call by Animation event only please! </summary>
		public void ANMTriggerNextSceneLoad()
		{
			if( _nextSceneFaderPrefab )
			{
				var op = Instantiate( _nextSceneFaderPrefab );
				DontDestroyOnLoad( op );
			}
			SceneManager.LoadScene( _nextScene );
		}

		/// <summary> PROTOTYPING FUNCTION </summary>
		public void Skip( float seconds )
		{
			if( !_isPlayingAnthemSequence ) return;
			var newTime = AnthemPlayer.time +seconds;
			newTime = Mathf.Clamp( newTime, StartTime, AnthemPlayer.clip.length );
			AnthemPlayer.time = newTime;
			AlignAnimationWithSound();
		}



		void Awake()
		{
			_anm = GetComponent<Animator>();
			_animation = GetComponent<Animation>();
			_skipConfirmWindow.SetActive( false );
		}


		IEnumerator Start()
		{
			_incomingLaunchAnthem = FindObjectOfType<LaunchAnthemLocator>();
			if( _incomingLaunchAnthem )
			{
				// Debug.Log( "FOUND INCOMING LAUNCHEVENT" );
				Destroy( AnthemPlayer.gameObject );
				AnthemPlayer = _incomingLaunchAnthem.GetComponent<AudioSource>();
			}
			else
			{
				yield return null;
				AnthemPlayer.time = StartTime;
				AnthemPlayer.Play();
				DontDestroyOnLoad( AnthemPlayer );
				AnthemPlayer.gameObject.AddComponent<AutoDestructEffect>();
			}
		}

		void OnEnable()
		{
			_skipButton.onClick.AddListener( SkipRequest );
			_skipConfirmButton.onClick.AddListener( SkipConfirm );
			_skipCancelButton.onClick.AddListener( SkipCancel );
		}

		void OnDisable()
		{
			_skipButton.onClick.RemoveListener( SkipRequest );
			_skipConfirmButton.onClick.RemoveListener( SkipConfirm );
			_skipCancelButton.onClick.RemoveListener( SkipCancel );
		}

		void Update()
		{
			if( Input.GetKeyDown( _skipKey ) )
				if( _skipButton.gameObject.activeInHierarchy ) SkipRequest();
				else if( _skipConfirmWindow.activeInHierarchy ) SkipConfirm();
			if( Input.GetKeyDown( _cancelKey ) && _skipConfirmWindow.activeInHierarchy ) SkipCancel();
		}

		void AlignAnimationWithSound()
		{
			if( _anm )
			{
				// var length = _anm.GetCurrentAnimatorStateInfo(0).length;
				// var newTime = (_anthemPlayer.time-_startTime)/length;
				_anm.CrossFadeInFixedTime( "GridFlightEntry", 0, 0, (AnthemPlayer.time-StartTime) );
			}
		}

		void SkipRequest()
		{
			_skipConfirmWindow.SetActive( true );
			_skipButton.gameObject.SetActive( false );
		}

		void SkipCancel()
		{
			_skipConfirmWindow.SetActive( false );
			_skipButton.gameObject.SetActive( true );
		}

		void SkipConfirm()
		{
			Destroy( AnthemPlayer.gameObject );
			SceneManager.LoadScene( _nextScene );
		}
	}
}

