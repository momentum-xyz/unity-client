using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Nextensions;

namespace HS
{
    /// <summary> Should be on all members of the Clickables layer. Reacts to
    /// Hover messages. </summary>
    public class Clickable : MonoBehaviour, IClickable
    {
        #region STATIC
        public static bool HasSelection => _currentHover != null;
        public static Clickable Selection => _currentHover;
        public static bool SelectedIsPlatformElement => _currentHover?._driver;
        public static string SelectedTag => HasSelection ? _currentHover.tag : "";
        public static string SelectedClickTag => HasSelection ? _currentHover.GetTagFromName() : "";
        public static bool SelectedIsAvatar => _currentHover?._avatar; // SelectedTag == "Avatar";
        public static bool SelectedIsPlasmaball => _currentHover?._plasmaball; // SelectedTag.Contains( "Plasmaball" );

        public static AvatarDriver SelectedAvatar => _currentHover?._avatar;
        public static PlasmaballDriver SelectedPlasmaball => _currentHover?._plasmaball; // this could go (UserInteraction can directly probe ReacMessage and only needs SelectedIsPlasmaball)

        // for testing
        public static GameObject PlatformObject => _currentHover?._driver?.gameObject ?? null;


        // IClickable, this label will be send to
        public string ClickLabel = "dashboard";


        // public static bool SelectedIsScreen => SelectedTag.Contains( "Screen" );
        // public static bool SelectedIsScreenLeft => SelectedTag == "ScreenLeft";
        // public static bool SelectedIsScreenMiddle => SelectedTag == "ScreenMiddle";
        // public static bool SelectedIsScreenRight => SelectedTag == "ScreenRight";
        // public static AvatarDriver SelectedAvatar => SelectedIsAvatar ? _currentHover.GetComponent<AvatarDriver>() : null;
        // public static PlasmaballDriver SelectedPlasmaball => SelectedIsPlasmaball ? _currentHover.GetComponent<PlasmaballDriver>() : null;
        // public static TeamSpaceDriver SelectedTeamSpace{get{
        // 	if( !SelectedIsScreen && !SelectedIsPlasmaball ) return null;
        // 	return _currentHover.GetComponentInParent<TeamSpaceDriver>();
        // }}
        // public static TrackSpaceDriver SelectedTrackSpace{get{
        // 	if( !SelectedIsScreen && !SelectedIsPlasmaball ) return null;
        // 	if( SelectedTeamSpace != null ) return null; // sanity
        // 	return _currentHover.GetComponentInParent<TrackSpaceDriver>();
        // }}
        // public static UserPlatformDriver SelectedPlatform{get{
        // 	if( !SelectedIsScreen && !SelectedIsPlasmaball ) return null;
        // 	if( SelectedTeamSpace != null ) return null; // sanity, in case platforms are childs of eachother
        // 	if( SelectedTrackSpace != null ) return null; // sanity, in case platforms are childs of eachother
        // 	return _currentHover.GetComponentInParent<UserPlatformDriver>();
        // }}


        static HashSet<Clickable> _members = new HashSet<Clickable>();
        static Clickable _currentHover;
        // static TeamSpaceDriver _currentTeamSpace;
        // static TrackSpaceDriver _currentTrackSpace;


        /// <summary> Expects a tag marked with @. Like "@screenLeft". Case-insensitive. </summary>
        public static bool IsTagged(string tag)
        {
            if (!_currentHover) return false;
            if (!tag.Contains("@")) return false;
            return _currentHover.gameObject.name.ToUpper().Contains(tag.ToUpper());
        }


        /// <summary> Turns off all hoverers except the given one. Feed it null 
        /// to turn them all off. </summary>
        public static void UpdateHover(Clickable hoverer = null)
        {
            if (_currentHover == hoverer) return;
            foreach (var elm in _members) elm.Hover(hoverer == elm);
            _currentHover = hoverer;
        }
        #endregion


        [SerializeField] GameObject _hoverObject;
        [SerializeField] UnityEvent _onClick;

        public UserPlatformDriver Driver
        {
            get
            {
                return _driver;
            }

            set { }
        }
        UserPlatformDriver _driver;
        AvatarDriver _avatar;
        PlasmaballDriver _plasmaball;

        void Hover(bool isHovering)
        {
            if (_hoverObject != null) _hoverObject.SetActive(isHovering);
        }

        public void Click()
        {
            _onClick?.Invoke();
        }

        /// <summary>
        /// Gets the Tag label from the name of the GameObject, it should start with "@", if not it will return Untagged
        /// </summary>
        /// <returns></returns>
        public string GetTagFromName()
        {
            string[] tagSplit = gameObject.name.Split('@');
            return tagSplit.Length > 1 ? tagSplit[1] : "Untagged";
        }

        void Awake()
        {
            if (!_hoverObject)
                _hoverObject = gameObject.FindByTaggedName("@hover");

            if (!GetComponent<Collider>() && GetComponent<MeshFilter>() && GetComponent<MeshFilter>().sharedMesh)
            {
                var box = gameObject.AddComponent<BoxCollider>();
                box.isTrigger = true;
                var bb = GetComponent<MeshFilter>().sharedMesh.bounds;
                box.center = bb.center;
                box.size = bb.size;
            }

            _members.Add(this);
            Hover(false);

            _plasmaball = GetComponent<PlasmaballDriver>();

            if (!_plasmaball) _avatar = GetComponent<AvatarDriver>();


            // LEGACY: old structures worked with Unity tags. We're translating them to object-name tags here
            switch (tag)
            {
                case "ScreenLeft": gameObject.name = gameObject.name.Tag("@ScreenLeft"); break;
                case "ScreenMiddle": gameObject.name = gameObject.name.Tag("@ScreenMiddle"); break;
                case "ScreenRight": gameObject.name = gameObject.name.Tag("@ScreenRight"); break;
            }


            // if we're not an avatar, we can assume we're either a platform or an element of a platform.
            if (!_avatar)
            {
                _driver = GetComponentInParent<UserPlatformDriver>();
            }
        }

        public string GetLabel()
        {
            return ClickLabel;
        }

        void OnDestroy()
        {
            _members.Remove(this);
        }
    }
}
