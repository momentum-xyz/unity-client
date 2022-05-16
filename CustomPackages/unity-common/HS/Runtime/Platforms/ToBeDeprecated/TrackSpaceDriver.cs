using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace HS
{
	public class TrackSpaceDriver : MonoBehaviour
	{
		[Header( "API" )]
		public int TrackID;
		public int ChallengeID;
		/// <summary> Makes a new texture appear on screen 1 (NB: this one is in principle reserved for the sponsor logo, is already filled in inside the prefab!) </summary>
		public void NewScreen1( Texture2D texture ){
			if( Screen1 ) Screen1.material.SetTexture( 	"_BaseMap", texture );
		}
		/// <summary> Makes a new texture appear on screen 2 (dashboard) </summary>
		public void NewScreen2( Texture2D texture ){
			if( Screen2 ) Screen2.material.SetTexture( 	"_BaseMap", texture );
		}
		/// <summary> Makes a new texture appear on screen 3 (livestream?) </summary>
		public void NewScreen3( Texture2D texture ){
			if( Screen3 ) Screen3.material.SetTexture( 	"_BaseMap", texture );
		}
		/// <summary> Makes a new texture appear on the Meme screen (doesn't trigger the MadeAMeme event) </summary>
		public void NewTrackMeme( Texture2D texture ){
			if( TrackMeme ) TrackMeme.material.SetTexture( 	"_BaseMap", texture );
		}
		/// <summary> Makes a new texture appear on the Meme screen (doesn't trigger the MadeAMeme event) </summary>
		public void NewTrackPoster( Texture2D texture ){
			if( TrackPoster ) TrackPoster.material.SetTexture( 	"_BaseMap", texture );
		}

		[FormerlySerializedAs( "Accent1" )]
		public Color MainColor = Color.green *0.7f;
		[FormerlySerializedAs( "Accent2" )]
		public Color AccentColor = Color.red *0.7f;
		[Range(0,50)] public float TetherRingRadius = 12f;


		[Header( "Local Refs")]
		[Tooltip( "We will retrieve the badge from this renderer." )]
		[SerializeField] Renderer BadgeSource;
		[SerializeField] RawImage LogoScreenSource;
		[SerializeField] PlasmaTether Tether;
		[SerializeField] Renderer Screen1;
		[SerializeField] Renderer Screen2;
		[SerializeField] Renderer Screen3;
		[SerializeField] Renderer TrackMeme;
		[SerializeField] Renderer TrackPoster;

		[Header( "PROTO" )]
		public int NumberOfTeams = 5;

		public Texture2D Badge => BadgeSource.material.GetTexture( "_BaseMap" ) as Texture2D;


		/// <summary> STOPGAP SOLUTION for tethering old-style structures to new UserPlatformDriver. Assumes we're already 
		/// in the right position. Use alsoRotate to also automatically orient to the new parent space. </summary>
		public void SetParent( UserPlatformDriver platform, bool alsoRotate )
		{
			if( !Application.isPlaying ) return; // prevents prototyping from going awry
			if( platform == null ) return; // sanity
			Tether.Setup( gameObject, platform.gameObject );
			// Tether.Setup( this.transform, TetherRingRadius, platform.transform, platform.TetherRingRadius );

			if( alsoRotate )
			{
				var dir = platform.transform.position - transform.position;
				dir.y = 0;
				transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
			}
		}
		/// <summary> Special Setup for non-trackrelated spaces like Jedi Lounge and Mission Control.
		/// Will setup all internal colors/badges, and align itself to its current position.
		/// NB: position the object correctly before calling this.</summary>
		public void Setup( Transform Earth )
		{
			var dir = Earth.position-transform.position;
			dir.y = 0;
			transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
		}


		/// <summary> Needed to setup the Tether connection, as well as get all colors and badges etc in
		/// order internally. 
		/// NB: position the element correctly before calling this.</summary>
		public void Setup( Transform tetherTarget, float tetherRingRadius )
		{
			Tether.Setup( gameObject, tetherTarget.gameObject );
			// Tether.Setup( transform, TetherRingRadius, tetherTarget, tetherRingRadius );
			var dir = tetherTarget.position-transform.position;
			dir.y = 0;
			transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
		}

		/// <summary> Updates the Tether graphics. Use this after re-positioning. </summary>
		public void UpdateTether()
		{
			Tether.UpdateTetherGraphics();
		}


		void Start()
		{
			BadgeSource.gameObject.SetActive( false );
			LogoScreenSource.gameObject.SetActive( false );
			if( LogoScreenSource.texture != null ) NewScreen2( LogoScreenSource.texture as Texture2D );
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			NHelp.DrawGizmosCircle( transform.position, Vector3.up, TetherRingRadius );
		}
	}
}