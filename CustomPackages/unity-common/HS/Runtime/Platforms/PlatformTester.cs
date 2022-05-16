using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;



namespace HS
{
	public class PlatformTester : MonoBehaviour
	{
		[System.Serializable] public struct ImageAndRatioCombo{
			public Texture2D Texture;
			public Vector2 Ratio;
		}


		public float Delay = 1.5f;

		[Space]
		public bool SceneWide;
		
		[Header( "Screen/Poster/Badge content" )]
		public List<ImageAndRatioCombo> Elements;
		[Header( "Name content" )]
		public Texture2D NameTexture;
		public int NameTextureLines = 15;
		[Header( "Totem models" )]
		public List<GameObject> TotemPrefabs;


		UserPlatformDriver _driver;


		void Awake()
		{
			_driver = GetComponent<UserPlatformDriver>();
		}


		void OnEnable()
		{
			StartCoroutine( Run() );
		}

		void OnDisable()
		{
			StopAllCoroutines();
		}


		IEnumerator Run()
		{
			if( Application.isEditor == false )
			{
				Destroy(this);
				yield break;
			}

			if( !_driver && !SceneWide ) yield break;
			
			var allPlatforms = 
				SceneWide
					? new List<UserPlatformDriver>( FindObjectsOfType<UserPlatformDriver>() )
					: null;

			while( true )
			{
				yield return new WaitForSeconds( Delay );
				var elm = Elements[Random.Range(0,Elements.Count)];
				if( SceneWide ) _driver = allPlatforms.PickOne();
				_driver.SetPoster( _driver.GetPresentPosterTags.PickOne(), elm.Texture, elm.Ratio.x/elm.Ratio.y );
				
				yield return new WaitForSeconds( Delay );
				elm = Elements[Random.Range(0,Elements.Count)];
				if( SceneWide ) _driver = allPlatforms.PickOne();
				_driver.SetBadge( _driver.GetPresentBadgeTags.PickOne(), elm.Texture );
				
				yield return new WaitForSeconds( Delay );
				elm = Elements[Random.Range(0,Elements.Count)];
				if( SceneWide ) _driver = allPlatforms.PickOne();
				_driver.SetScreen( _driver.GetPresentScreenTags.PickOne(), elm.Texture );

				yield return new WaitForSeconds( Delay );
				int l = Random.Range(0,NameTextureLines);
				if( SceneWide ) _driver = allPlatforms.PickOne();
				_driver.SetNameRibbon(NameTexture,l,NameTextureLines);

				yield return new WaitForSeconds( Delay );
				var c1 = Random.ColorHSV(0,1,0.5f,0.9f,0.5f,0.9f);
				var c2 = Random.ColorHSV(0,1,0.5f,0.9f,0.5f,0.9f);
				if( SceneWide ) _driver = allPlatforms.PickOne();
				_driver.SetColors( new[]{c1,c2} );

				if( TotemPrefabs != null && TotemPrefabs.Count > 0 )
				{
					yield return new WaitForSeconds( Delay );
					var p = allPlatforms.PickOne();
					var t = p.GetPresentSlotTags.PickOne();
					if( p!=null && t!=null )
					{
						var op = Instantiate( TotemPrefabs.PickOne() );
						p.SlotObject( t, op );
					}
				}
			}
		}
	}
}