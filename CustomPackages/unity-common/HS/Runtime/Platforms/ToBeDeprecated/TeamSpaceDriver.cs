using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
    public class TeamSpaceDriver : MonoBehaviour
    {
        public int TeamID;
        public Guid teamGuid = Guid.Empty;
        public string sendToReact = "not set";

        [Header("Internal Refs")]
        public PlasmaballDriver Plasmaball;
        public PlasmaTether Tether;
        public ExhaustDriver Exhaust;
        [SerializeField] Renderer Screen1, Screen1a, Screen2, Screen3;
        [SerializeField] Renderer TeamName, TeamMeme, TeamPoster, TeamMeme2, TeamMeme3;             // NR hybrid
        [SerializeField] Renderer TrackBadge;
        [SerializeField] Renderer Floor;
        //[SerializeField] string FloorMaterialName = "teamPlatformFloor";
        //[SerializeField] string BaseMetalName = "BaseMetal";

        [Range(0,50)] public float TetherRingRadius = 5f;

    
        /// <summary> Used internally to pickup track colors </summary>
        public TrackSpaceDriver Track => _track;
        TrackSpaceDriver _track;

        public bool HasBeenSetup{get;private set;}




        /// <summary> Makes a new texture appear on screen 1 (achievements) </summary>
        public void NewScreen1( Texture2D texture ){
            if( Screen1 ) Screen1.material.SetTexture( 	"_BaseMap", texture );
        }


        /// <summary> Makes a new texture appear on screen 1a (problem) </summary>
        public void NewScreen1a(Texture2D texture)
        {
            if (Screen1a) Screen1a.material.SetTexture("_BaseMap", texture);
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
        public void NewTeamMeme( Texture2D texture ){
            if( TeamMeme ) TeamMeme.material.SetTexture( 	"_BaseMap", texture );
        }
        /// <summary> Makes a new texture appear on the poster screen (doesn't trigger the MadeAPoster event)</summary>
        public void NewTeamPoster( Texture2D texture ){
            if( TeamPoster ) TeamPoster.material.SetTexture( "_BaseMap", texture );
        }
        /// <summary> Set a texture for the team name</summary>
        public void NewTeamName( Texture2D texture ){
            if( TeamName ) 
            {
                TeamName.material.mainTexture = texture;
                if( TeamName.material.HasProperty( "_BaseMap" ) ) TeamName.material.SetTexture( 	"_BaseMap", texture );
                // if( TeamName.material.HasProperty( "_MainTex" ) ) TeamName.material.SetTexture( 	"_MainTex", texture );
            }
        }
        /// <summary> Animates the rocket engine's flame to a new setting </summary>
        public void SetExhaustBoost( float normalizedBoostPower ) =>
            Exhaust.SetBoost( normalizedBoostPower );


        [Header("Prototyping")]
        public TrackSpaceDriver TestTrackSpace;


        // [ContextMenu( "Setup Trackspace Connection" )]
        // /// <summary> This overload is for prototyping only! </summary>
        // void SetupTrackspaceConnection() => SetupTrackspaceConnection( TestTrackSpace );


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
        /// <summary> LEGACY: for now, you can use SetParent( UserPlatformDriver) to tether this space to 
        /// a UserPlatformDriver.
        /// <br/> Sets up the teamspace as a 'member'of given track, picking up its colors, badge etc. It also
        /// makes a tether connection to it, but it assumes it's already in the right locaktion (doesn't move to
        /// accomodate the TrackSpace) </summary>
        public void SetupTrackspaceConnection( TrackSpaceDriver targetTrackSpace )
        {
            if( !Application.isPlaying ) return; // prevents prototyping from going awry
            if( targetTrackSpace == null ) return; // sanity

            _track = targetTrackSpace;

			Tether.Setup( gameObject, targetTrackSpace.gameObject );
            // Tether.Setup( this.transform, TetherRingRadius, targetTrackSpace.transform, targetTrackSpace.TetherRingRadius );

            var dir = targetTrackSpace.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation( dir, Vector3.up );

            if( TrackBadge ) TrackBadge.material.SetTexture( "_MainTex", targetTrackSpace.Badge );
        
            HasBeenSetup = true;
            
            /*
            // make some materials match track colors
            var mat = Floor.materials.FirstOrDefault<Material>( elm => elm.name.Contains( FloorMaterialName ) );
            var baseMetalMat = Floor.materials.FirstOrDefault<Material>( elm => elm.name.Contains( BaseMetalName ) );
            if( mat )
            {
                mat.SetColor( "_Albedo1", targetTrackSpace.MainColor );
                mat.SetColor( "_NeonTint", targetTrackSpace.AccentColor );
            }
            if( baseMetalMat )
                baseMetalMat.SetColor( "_BaseColor", targetTrackSpace.MainColor*baseMetalMat.GetColor( "_BaseColor" ) );
                */
        }

        /// <summary> Updates the Tether graphics. Use this after re-positioning. </summary>
        public void UpdateTether()
        {
            Tether.UpdateTetherGraphics();
        }


        // void Start()
        // {
        // 	// PROTO!
        // 	SetupTrackspaceConnection( TestTrackSpace );
        // }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            NHelp.DrawGizmosCircle( transform.position, Vector3.up, TetherRingRadius );
        }
    }
}