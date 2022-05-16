using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
	public class SkyboxManager : MonoBehaviour
	{
		#region static
		static SkyboxManager _instance;

		public static System.Action OnChange;
		public static string CurrentSkyboxName => _idx <_count ? _names[_idx] : "[unknown]";
		public static Cubemap CurrentSkyboxCubemap{get;set;}

		static List<string> _names = new List<string>();
		static List<GameObject> _objects = new List<GameObject>();
		static int _count => _objects.Count;
		static int _idx;

		/// <summary> Rotates through skyboxes </summary>
		public static void Previous()
		{	
			if( !_instance ) return;
			SetSkybox( (_idx-1+_count)%_count );
		}
		/// <summary> Rotates through skyboxes </summary>
		public static void Next()
		{
			if( !_instance ) return;
			SetSkybox( (_idx+1)%_count );
		}
		/// <summary> Set Skybox by name </summary>
		public static bool SetSkybox( string name )
		{
			if( !_instance ) return false;
			var newBox = _names.FirstOrDefault( a=> a.ToLower() == name.ToLower() );
			if( newBox == "" ) return false;
			return SetSkybox( _names.IndexOf(newBox) );
		}

		/// <summary> Set Skybox by index </summary>
		public static bool SetSkybox( int idx )
		{
			if( !_instance ) return false;
			idx = Mathf.Clamp( idx, 0, _count-1 );
			if( idx == _idx ) return true;
			_idx = idx;
			_instance.SetVisibilities();
			return true;
		}

		#endregion

		[Tooltip( 	"All children of the SkyboxManager are assumed to be skyboxes that can be "+
					"enabled and disabled, in order. List the elements that are to be ignored here." )]
		[SerializeField] List<GameObject> _childIgnore;



		void SetVisibilities()
		{
			// Inside the active skybox object that gets activated here, there should be
			// an object containing a SetSkybox behaviour. That takes care both of setting the
			// correct skybox, AND communicating back to us which cubemap is used
			// (it sets CurrentSkybox).
			for( int i = 0; i < _objects.Count; i++ )
				_objects[i].SetActive( i == _idx );
			OnChange?.Invoke();
		}		


		void Awake()
		{
			_instance = this;
			_objects.Clear();
			_names.Clear();
			for( int i = 0; i < transform.childCount; i++ )
			{
				var op = transform.GetChild( i ).gameObject;
				if( !_childIgnore.Contains( op.gameObject ) )
				{
					_objects.Add( op );
					_names.Add( op.gameObject.name );
					op.SetActive( false );
				}
			}
			_idx = Mathf.Clamp( _idx, 0, _objects.Count-1 );
		}


		void Start()
		{
			SetVisibilities();
		}


		void OnDestroy()
		{
			if( _instance == this )
			{
				_objects.Clear();
				_names.Clear();
				_instance = null;
				CurrentSkyboxCubemap = null;
			}
		}
	}
}