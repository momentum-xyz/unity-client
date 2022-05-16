using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;



namespace HS
{
	public class PickupTrackColors : MonoBehaviour
	{
		[SerializeField] List<Renderer> _renderers;
		[SerializeField] Material _floorMaterial;
		[SerializeField] List<Material> _tintMaterials;
		[SerializeField] List<Material> _accentMaterials;

		TrackSpaceDriver _driver;

		bool _didRun;

		void OnEnable()
		{
			if( !_didRun ) StartCoroutine( RunGrab() );
		}


		IEnumerator RunGrab()
		{
			var tintProperties = new List<string>{
				"_BaseColor",
				"_Tint",
				"_MainColor"
			};

			// If we're attached to a team space, we'll need to  check (and possibly wait) 
			// until that one has been initialized with the correct track connection.
			// If, on the other hand, we're connected to a trackspace directly, we can
			// immediately pickup the colors.
			var team = GetComponentInParent<TeamSpaceDriver>();
			if( team )
			{
				yield return new WaitUntil( ()=> team.HasBeenSetup );
				_driver = team.Track;
			}
			else 
				_driver = GetComponentInParent<TrackSpaceDriver>();


			foreach( var r in _renderers )
			{
				for( int i = 0; i < r.sharedMaterials.Length; i++ )
				{
					// Somehow, the sharedMaterial of the floroMaterial here is already an instance,
					// so we need a special check, by removing ' (Instance)' from the namestring
					var floorNameString = r.sharedMaterials[i].name.Replace( " (Instance)", "" );
					if( _floorMaterial != null && floorNameString == _floorMaterial.name )
					{
						r.materials[i].SetColor( "_Albedo1", _driver.MainColor );
						r.materials[i].SetColor( "_NeonTint", _driver.AccentColor );
					}
					else if ( _tintMaterials.Exists( elm => 
									elm != null 
									&& elm.name.Replace( " (Instance)", "" ) == r.sharedMaterials[i].name.Replace( " (Instance)", "" ) ) )
					{
						foreach( var prop in tintProperties )
							if( r.materials[i].HasProperty( prop ) )
								r.materials[i].SetColor( prop, _driver.MainColor*r.sharedMaterials[i].GetColor( prop ) );
					}
					else if ( _accentMaterials.Exists( elm => 
									elm != null 
									&& elm.name.Replace( " (Instance)", "" ) == r.sharedMaterials[i].name.Replace( " (Instance)", "" ) ) )
					{
						foreach( var prop in tintProperties )
							if( r.materials[i].HasProperty( prop ) )
								r.materials[i].SetColor( prop, _driver.AccentColor*r.sharedMaterials[i].GetColor( prop ) );
					}
				}
			}
			_didRun = true;
		}




	}
}