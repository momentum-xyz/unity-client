using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HS
{
	public class AvatarDriver : MonoBehaviour
	{
		[Header( "API" )]
		public int AvatarID;
		/// <summary> Used to access the transform of the moving bit of the avatar (usually you'll
		/// simply want to use the steady root object though) </summary>
		public WanderHover AvatarHover;
		/// <summary> Contains the role that this prefab has been set to. Each role has a seperate LOD0
		/// prefab, so we can balance the colors in there. Use the static 
		/// HS.AvatarDriver.RoleColor( role ) to get the preferred tint of the 
		/// role (meant for the lower LOD avatar displays).</summary>
		public AvaRole RoleReadout;


		// [Header( "params" )]
		//[SerializeField][Range(0,1)] float InnerOrbitRadius = 0.09f;
		
		// local refs
		[Header( "refs" )]
		[SerializeField] GameObject AvatarData;
		[SerializeField] GameObject _involvementEffect;
		[SerializeField] TMP_Text AvatarName;
		[SerializeField] TMP_Text AvatarOrganisation;
		[SerializeField] TMP_Text AvatarRole;
		[SerializeField] RawImage Badge;



		/// <summary> turn the name/role/badge/organisation display on or off </summary>
		public void SetDataVisibility( bool visible ) 		=> AvatarData.SetActive( visible );
		/// <summary> Set which name to display for this avatar. </summary>
		public void SetAvatarName( string name )            => AvatarName.text = name;
		/// <summary> Set which organisation to display for this avatar. [discuss: will there sometimes be -no- org?]</summary>
		public void SetAvatarOrganisation( string name )    => AvatarOrganisation.text = name;
		/// <summary> Set which role to display for this avatar. [discuss: will there sometimes be -no- role?]
		/// NB: each role also has an associated color! You can set that via SetAvatarColor( HS.AvaRole role )</summary>
		public void SetAvatarRole( string name )            => AvatarRole.text = name;
		/// <summary> Set which badge to display for this avatar. [discuss: will there sometimes be -no- badge?] </summary>
		public void SetBadge( Texture2D imageWithAlpha )    => Badge.texture = imageWithAlpha;
		/// <summary> Displays a 'chatty' effect to denote that the avatar is engaged in some chat (not meant as a 'do not disturb' notif) </summary>
		public void SetAvatarInvolvement( bool involved )	=> _involvementEffect?.SetActive( involved );


		/// <summary> The preferred tints of the various avatar roles </summary>
		public static Color RoleColor( AvaRole role ) => AvatarRolesAndColors.GetColor( role );

		// void Update()
		// {
		// 	AvatarHover.Radius = InnerOrbitRadius;
		// }
		void Awake()
		{
			_involvementEffect.SetActive( false );
		}
	}
	
	
	public enum AvaRole{
		Spectator = 0, 
		TeamMember,
		Jedi,
		TrackMajor,
		ChallengeLead,
		DelegationMember,
		CrewMember,
		User
	}


}