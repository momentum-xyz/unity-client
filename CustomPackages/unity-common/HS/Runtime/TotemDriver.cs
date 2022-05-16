using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    /// <summary> Takes care of reflecting its track badge and colors </summary>
    public class TotemDriver : MonoBehaviour
    {
        [SerializeField] Renderer ShellBadgeRenderer;
        [SerializeField] Material ShellBadgeMaterial;
        [SerializeField] Renderer NeonTint;

        TrackSpaceDriver _trackDriver;
		UserPlatformDriver _platform;


        void Start()
        {
            _trackDriver = GetComponentInParent<TrackSpaceDriver>();
			_platform = GetComponentInParent<UserPlatformDriver>();
            Setup();
        }
        void Setup()
        {
			var badge = _platform?.Badge ?? _trackDriver?.Badge;
			if( !badge ) return;
            for( int i = 0; i < ShellBadgeRenderer.sharedMaterials.Length; i++ )
                if( ShellBadgeRenderer.sharedMaterials[i] == ShellBadgeMaterial )
                {
                    ShellBadgeRenderer.materials[i].SetTexture( "_BaseMap", badge );
                    ShellBadgeRenderer.materials[i].SetTexture( "_EmissionMap", badge );
                }

			var neonColor = _platform ? _platform.AccentColor : _trackDriver? _trackDriver.AccentColor : Color.white;
			if( neonColor == Color.white ) return;
            NeonTint.material.SetColor( "_MainColor", neonColor );
        }
    }
}