using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class ChapterManager : MonoBehaviour
	{
		public static string ActiveChapter => $"[{_idx}] {((Chapter)_idx).ToString()}";
		static ChapterManager _instance;
		static int _chapterCount = 9; // should be count of NON-NEGATIVE chapters
		static int _idx = 0;

		public enum Chapter{
			WaitingForLaunch 			= -1,
			Arrival 					= 0,
			ShowtimeGoGoGo				= 1,
			WeveGotThisOrDoWe			= 2,
			ChallengesAndTemptations	= 3,
			Abyss						= 4,
			Transformation				= 5,
			GoGoGoAllOut				= 6,
			HurryUpAndFinish			= 7,
			TimeIsUp					= 8
		}


		public static void Previous()
		{
			SetChapter( (_idx-1+_chapterCount)%_chapterCount );
		}
		public static void Next()
		{
			SetChapter( (_idx+1)%_chapterCount );
		}

        /// <summary> Set the current chapter, enummed version </summary>
        public static void SetChapter( Chapter newChapter ) =>
			SetChapter( (int)newChapter );
		/// <summary> Set the current chapter, int version </summary>
		public static void SetChapter( int newChapter )
		{
			if( newChapter == _idx ) return;
			_idx = Mathf.Clamp( newChapter, 0, _chapterCount-1 );
			if( !_instance ) return;
			_instance.SetVisibilities();
		}


		public static System.Action OnChange;
		public static Cubemap CurrentSkybox{get;set;}


		[SerializeField] List<GameObject> _childIgnore;

		List<GameObject> _chapters = new List<GameObject>();


		void SetVisibilities()
		{
			for( int i = 0; i < _chapters.Count; i++ )
				_chapters[i].SetActive( i == _idx );
			OnChange?.Invoke();
		}		


		void Awake()
		{
			_instance = this;
			_chapters.Clear();
			_idx = Mathf.Clamp( _idx, 0, transform.childCount );
			for( int i = 0; i < transform.childCount; i++ )
			{
				var op = transform.GetChild( i ).gameObject;
				if( !_childIgnore.Contains( op.gameObject ) )
				{
					_chapters.Add( op );
					op.SetActive( false );
				}
			}
		}


		void Start()
		{
			SetVisibilities();
		}


		void OnDestroy()
		{
			if( _instance == this )
				_chapters.Clear();
		}
	}
}