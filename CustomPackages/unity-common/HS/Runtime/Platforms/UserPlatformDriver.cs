using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;


namespace HS
{
    public class UserPlatformDriver : MonoBehaviour
    {

        [HideInInspector] public Guid guid; // world object/structure guid

        // could be needed by others
        [Header("publics")]
        [Range(0, 50)] public float TetherRingRadius = 5f;


        // internal references
        [Header("Internal Refs")]
        [SerializeField] protected HashSet<PlasmaballDriver> _plasmaBalls = new HashSet<PlasmaballDriver>();
        [SerializeField] protected PlasmaTether _tether;
        [SerializeField] protected ForceFieldDriver _forceField;
        [SerializeField] protected StageModeDriver _stageMode;
        [SerializeField] protected Dictionary<string, HashSet<CustomContentRenderer>> _screens = new Dictionary<string, HashSet<CustomContentRenderer>>();
        [SerializeField] protected Dictionary<string, HashSet<CustomContentRenderer>> _badges = new Dictionary<string, HashSet<CustomContentRenderer>>();
        [SerializeField] protected Dictionary<string, HashSet<IPosterRenderer>> _posters = new Dictionary<string, HashSet<IPosterRenderer>>();
        [SerializeField] protected Dictionary<string, HashSet<Transform>> _slots = new Dictionary<string, HashSet<Transform>>();
        [SerializeField] protected Dictionary<string, HashSet<GameObject>> _slottedObjects = new Dictionary<string, HashSet<GameObject>>();

        protected PlasmaTetherAttachShape _tetherShape;

        public Texture2D Badge { get; private set; }
        public Color[] Colors { get; private set; }
        public Color MainColor => Colors != null && Colors.Length > 0 ? Colors[0] : Color.red;
        public Color AccentColor => Colors != null && Colors.Length > 1 ? Colors[1] : Color.cyan;

        //
        // API
        //



        /// <summary> Not Yet Implemented </summary>
        public bool HasBeenSetup { get; private set; }


        /// <summary> LEGACY: set the tether to a trackspace. </summary>
        public void SetParent(TrackSpaceDriver trackspace)
        {
            _tether.Setup(gameObject, trackspace.gameObject);
            // _tether.Setup( transform, TetherRingRadius, trackspace.transform, trackspace.TetherRingRadius );
        }
        /// <summary> Sets the platform's parent platform, to attach its
        /// tether to. We expect the position to be already set correctly. If not, make sure you UpdateTether() during motion. 
        /// <br/> Feed it null for top-level platforms. It will tether itself (for now) to a default ring on Y=0.
        /// <br/> NOTE: the space won't change its hierarchical state, only remember which object acts as parent for tethering.</summary>
        public void SetParent(UserPlatformDriver platform)
        {
            if (_tether == null) return;

            _tether.Setup(gameObject, platform ? platform.gameObject : null);

            // if( _tetherShape ) _tether.Setup( _tetherShape, platform );
            // else if ( platform && platform.TetherShape ) _tether.Setup( transform, TetherRingRadius, platform.TetherShape );
            // else if ( )

            // float endRad = platform != null ? platform.TetherRingRadius : 283 ; // MAGIC: default radius for attaching to world space, holdover from orig-momentum days (it's the world rings radius)
            // _tether.Setup( transform, TetherRingRadius, platform.transform, endRad );
        }


        /// <summary> Sets the platform's custom colors. For now we only expect two of them (main color and accent color) 
        /// TODO: IMPLEMENT: If inherit is true, the child platforms will also pickup the colors. </summary>
        public void SetColors(Color[] colors, bool inherit = false)
        {
            Colors = colors.Clone() as Color[];
            foreach (var c in Colorables) c.Set(colors);
        }

        /// <summary> TODO: IMPLEMENT: Resets the custom colors to the defaults. For now we only expect two of them (main color and accent color) 
        /// If inherit is true, the child platforms will also pickup the colors. </summary>
        public void ResetColors(Color[] colors, bool inherit = false)
        {
            // foreach( var c in Colorables ) c.Reset(colors);
        }



        /// <summary>
        /// All the LODSets inside this platform. Usually only one, but some big spaces may contain
        /// multiple.
        /// </summary>
        public List<LODSet> LODSets { get { if (_LODSets == null) FindLODSets(); return _LODSets; } }
        List<LODSet> _LODSets = null;
        void FindLODSets()
        {
            // NB: we need to find the LODSets that are part of -this- platform, not the ones of
            // possible child spaces
            _LODSets = new List<LODSet>();
            foreach (var l in GetComponentsInChildren<LODSet>(true))
            {
                if (l.GetComponentInParent<UserPlatformDriver>() == this) // we're not a child of another UserPlatformDriver
                                                                          // extra LEGACY checks
                    if (
                        (l.GetComponentInParent<TeamSpaceDriver>() == null || l.GetComponentInParent<TeamSpaceDriver>().GetComponentInParent<UserPlatformDriver>() != this)
                        &&
                        (l.GetComponentInParent<TrackSpaceDriver>() == null || l.GetComponentInParent<TrackSpaceDriver>().GetComponentInParent<UserPlatformDriver>() != this)
                    )

                        _LODSets.Add(l);


            }
        }


        /// <summary> Set the labeled user surface to the given texture. 
        /// Current labels:
        /// @Video			-> screen
        /// @Description	-> screen
        /// @Problem		-> screen
        /// @Solution		-> screen
        /// @Poster			-> poster
        /// @Meme			-> badge
        /// @Name			-> nameRibbon
        /// (User Surfaces can be Screens (always 4x3), Posters (will adjust to the texture's ratio), 
        /// Badges (these respect the alpha channel) and Names (will appear on a space's name ribbon))
        /// </summary>
        public void SetTexture(string tag, Texture2D texture, float ratio)
        {
            if (tag.ToUpper().Contains("VIDEO"))
            {
                SetScreen("@right", texture);
            }
            else if (tag.ToUpper().Contains("DESCRIPTION"))
            {
                SetScreen("@middle", texture);
                SetScreen("@mid", texture);
            }
            else if (tag.ToUpper().Contains("PROBLEM") || tag.ToUpper().Contains("THIRD"))
            {
                SetScreen("@left", texture);
                SetScreen("@left2", texture);
            }
            else if (tag.ToUpper().Contains("SOLUTION"))
            {
                SetScreen("@left1", texture);
            }

            else if (tag.ToUpper().Contains("POSTER"))
            {
                SetPoster("@banner", texture, ratio);
                SetPoster("@right", texture, ratio);
                SetPoster("@main", texture, ratio);
            }

            else if (tag.ToUpper().Contains("MEME"))
            {
                SetPoster("@top", texture, ratio);
                SetPoster("@left", texture, ratio);
                SetBadge("@meme", texture);
            }
            else if (tag.ToUpper().Contains("NAME")) SetNameRibbon(texture, 0, 1);
        }



        /// <summary> Sets the platform's name ribbon to the n-th name on the given texture. The texture is expected
        /// to contain [count] names horizontally. Default is 15. </summary>
        public void SetNameRibbon(Texture2D texture, int n, int count = 15)
        {
            foreach (var r in Ribbons) r.Set(texture, n, count);
        }


        /// <summary> Set the given (tagged) poster to display the given texture. 
        /// <para> Add a ratio as a third parameter if it is 
        /// known, else we will attempt to evaluate the ratio from the Texture2D's name string: if we can find 
        /// something like 1x2 or 15x9 or 22x365 in it, it'll treat that as the ratio</para>
        /// <para>If no ratio is found, we'll use a default ratio of 4x3 </para>
        /// <para>returns false if the tagged poster isnt present.</para>
        /// </summary>
        public bool SetPoster(string tag, Texture2D texture)
            => SetPoster(tag, texture, texture.name.PickupRatio(3f / 4f));
        /// <summary> Set the given (tagged) poster to display the given texture, and adjust to the given ratio. 
        /// <para>returns false if the tagged poster isnt present.</para>
        /// </summary>
        public bool SetPoster(string tag, Texture2D texture, float ratio)
        {
            if (_posters == null || _posters.Count == 0) return false;
            if (_posters.ContainsKey(tag.ToUpper()) == false) return false;
            foreach (var p in _posters[tag.ToUpper()]) p?.Set(texture, ratio);
            return true;
        }
        /// <summary> Set the given (tagged) screen to display the given texture. Screen textures are displayed at a ration of 4:3. 
        /// returns false if the tagged screen isnt present. </summary>
        public bool SetScreen(string tag, Texture2D texture)
        {
            if (_screens == null || _screens.Count == 0) return false;
            if (_screens.ContainsKey(tag.ToUpper()) == false) return false;
            foreach (var s in _screens[tag.ToUpper()]) s?.Set(texture);
            return true;
        }
        /// <summary> Set the given (tagged) badge to display the given texture. Badge textures can contain an alpha channel and should 
        /// be square (1:1). returns false if the tagged badge isnt present. </summary>
        public bool SetBadge(string tag, Texture2D texture)
        {
            Badge = texture;
            if (tag == null) return false;
            if (_badges == null || _badges.Count == 0) return false;
            if (_badges.ContainsKey(tag.ToUpper()) == false) return false;
            foreach (var b in _badges[tag.ToUpper()]) b?.Set(texture);
            return true;
        }
        /// <summary> Slot the given object into the platform slot tagged with the given name.
        /// (fi, if a platform has (a) slot(s) called @SlotTotem, you can call this with SlotObject("Totem",object) ).
        /// Use GetPresentSlotTags to recieve a list of found slot tags.
        /// If there's multiple slots found, the object will be automatically instantiated. 
        /// If there's already objects present, they will be destroyed.
        /// </summary>
        public bool SlotObject(string tag, GameObject slottedObject)
        {
            tag = tag.ToUpper();
            if (!_slots.ContainsKey(tag)) return false;
            if (!_slottedObjects.ContainsKey(tag)) _slottedObjects.Add(tag, new HashSet<GameObject>());
            foreach (var op in _slottedObjects[tag]) Destroy(op);
            _slottedObjects[tag].Clear();

            var cnt = 0;
            foreach (var t in _slots[tag.ToUpper()])
            {
                var op = slottedObject;
                if (cnt > 0) op = Instantiate(slottedObject);

                op.transform.SetParent(t, true);
                op.transform.Zero();
                _slottedObjects[tag].Add(op);
                op.SetActive(true);
            }

            return true;
        }
        /// <summary> Destroys any present slotted objects, optionally with the given slot tag.</summary>
        public bool DestroySlottedObjects(string tag = "")
        {
            var cnt = 0;
            foreach (var key in _slottedObjects.Keys)
                if (tag == "" || tag.ToUpper() == key)
                {
                    foreach (var op in _slottedObjects[key]) { Destroy(op); cnt++; }
                    _slottedObjects[key].Clear();
                }
            return cnt > 0;
        }



        /// <summary> Sets both the visual indication of the meeting-room shielding (displayed when the platform's meeting room is set to
        /// private/limited access) and the rollover for the plasmaball (which depends on if the current user is allowed in).
        /// <br/> NOTE: the player can still click on the plasmaball, and messages will still be sent, even when they aren't actually
        /// allowed in. Checks should be made when this happens! </summary>
        public void SetPrivacy(bool meetingroomIsPrivate, bool currentUserIsAllowedIn)
        {
            if (_forceField != null) _forceField.gameObject.SetActive(meetingroomIsPrivate);
            if (_plasmaBalls != null) foreach (var p in _plasmaBalls) p.SetPrivate(!currentUserIsAllowedIn);
        }
        public bool Privacy => _forceField != null ? _forceField.gameObject.activeSelf : false;



        /// <summary> Displays the platform's "Stage Mode" indicator
        /// </summary>
        public void SetStageMode(bool active)
        {
            if (_stageMode) _stageMode.gameObject.SetActive(active);
        }
        public bool StageMode => _stageMode == null ? false : _stageMode.gameObject.activeSelf;



        /// <summary> Lists the poster tags found inside the space. </summary>
        public List<string> GetPresentPosterTags => _posters == null ? null : _posters.Keys.ToList();
        /// <summary> Lists the screen tags found inside the space. </summary>
        public List<string> GetPresentScreenTags => _screens == null ? null : _screens.Keys.ToList();
        /// <summary> Lists the badge tags found inside the space. </summary>
        public List<string> GetPresentBadgeTags => _badges == null ? null : _badges.Keys.ToList();
        /// <summary> Lists the badge tags found inside the space. </summary>
        public List<string> GetPresentSlotTags => _slots == null ? null : _slots.Keys.ToList();
        /// <summary> Detects if there are any slotted objects present. </summary>
        public bool ContainsSlottedObjects()
        {
            foreach (var set in _slottedObjects.Values)
                if (set != null && set.Count > 0) return true;
            return false;
        }
        /// <summary> Detects if the given slot already contains objects. </summary>
        public bool ContainsSlottedObjects(string slotTag) => _slottedObjects.ContainsKey(tag.ToUpper()) && _slottedObjects[tag.ToUpper()].Count > 0;


        /// <summary> Updates the Tether graphics. Use this after re-positioning. </summary>
        public void UpdateTether() =>
            _tether?.UpdateTetherGraphics();




        //
        // STRUCTURE
        //

        /// <summary> Internal. </summary>
        public HashSet<NameRibbon> Ribbons = new HashSet<NameRibbon>();
        /// <summary> Internal. </summary>
        public HashSet<PlatformColors> Colorables = new HashSet<PlatformColors>();



        void Awake()
        {
            FindTaggedMembers();
            // if( !_plasmaBall ) _plasmaBall = GetComponentInChildren<PlasmaballDriver>(true);
            // if( !_forceField ) _forceField = GetComponentInChildren<ForceFieldDriver>(true);
            // if( !_tether ) _tether = GetComponentInChildren<PlasmaTether>( true );
            AutoFindInternalRefs();
            if (_forceField) _forceField.gameObject.SetActive(false);
            if (_stageMode) _stageMode.gameObject.SetActive(false);
            _tetherShape = GetComponentInChildren<PlasmaTetherAttachShape>(true);
        }


        [ContextMenu("Fill Internal Refs")]
        void AutoFindInternalRefs()
        {
            // if( !_plasmaBall ) _plasmaBall = GetComponentInChildren<PlasmaballDriver>(true);
            _plasmaBalls = new HashSet<PlasmaballDriver>(GetComponentsInChildren<PlasmaballDriver>(true));
            if (!_forceField) _forceField = GetComponentInChildren<ForceFieldDriver>(true);
            if (!_stageMode) _stageMode = GetComponentInChildren<StageModeDriver>(true);
            if (!_tether) _tether = GetComponentInChildren<PlasmaTether>(true);
        }



        void FindTaggedMembers()
        {

            _posters.Clear();
            foreach (var r in this.FindByTaggedNameAll("@poster"))
            {
                var type = (r.gameObject.name.SplitTag("@poster") ?? "default").ToUpper();
                if (_posters.ContainsKey(type) == false) _posters.Add(type, new HashSet<IPosterRenderer>());
                var poster = r.GetComponent<IPosterRenderer>();
                // since auto-assigned posters need to try and pickup their aspect ratio from 
                // their object name, we need to treat them differently
                if (poster == null)
                {
                    poster = r.gameObject.AddComponent<PosterRenderer>();
                    (poster as PosterRenderer).PickupRatio();
                }
                _posters[type].Add(
                    r.GetComponent<IPosterRenderer>() ?? r.gameObject.AddComponent<PosterRenderer>()
                );
            }
            _badges.Clear();
            foreach (var r in this.FindByTaggedNameAll("@badge"))
            {
                var type = (r.gameObject.name.SplitTag("@badge") ?? "default").ToUpper();
                if (_badges.ContainsKey(type) == false) _badges.Add(type, new HashSet<CustomContentRenderer>());
                _badges[type].Add(
                    r.gameObject.CreateOrAddComponent<CustomContentRenderer>()
                );
            }
            _screens.Clear();
            foreach (var r in this.FindByTaggedNameAll("@screen"))
            {
                var type = (r.gameObject.name.SplitTag("@screen") ?? "default").ToUpper();
                if (_screens.ContainsKey(type) == false) _screens.Add(type, new HashSet<CustomContentRenderer>());
                _screens[type].Add(
                    r.gameObject.CreateOrAddComponent<CustomContentRenderer>()
                );
            }
            _slots.Clear();
            foreach (var r in this.FindByTaggedNameAll("@slot"))
            {
                var type = (r.gameObject.name.SplitTag("@slot") ?? "default").ToUpper();
                if (_slots.ContainsKey(type) == false) _slots.Add(type, new HashSet<Transform>());
                _slots[type].Add(
                    r.transform
                );
            }
        }



        void OnDrawGizmosSelected()
        {
            if (GetComponentInChildren<PlasmaTetherAttachShape>()) return;
            Gizmos.color = Color.cyan;
            NHelp.DrawGizmosCircle(transform.position, Vector3.up, TetherRingRadius);
        }
    }
}
