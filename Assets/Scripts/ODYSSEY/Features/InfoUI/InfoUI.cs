using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Odyssey
{
    public enum PoolState
    {
        EMPTY,
        DOWNLOADING,
        ERROR,
        INIT
    }

    public class ElementPool
    {
        public PoolState State = PoolState.EMPTY;
        public GameObject Prefab { get { return _prefab; } set { _prefab = value; } }

        private GameObject _prefab;
        private Queue<InfoUIElement> _elements = new Queue<InfoUIElement>();
        private Transform _container;
        private int _initialCount = 3;

        public ElementPool(GameObject prefab, Transform container, int initialCount = 20)
        {
            _prefab = prefab;
            _container = container;
            _initialCount = initialCount;
        }

        public ElementPool(Transform container, int initialCount = 3)
        {
            _container = container;
            _initialCount = initialCount;
        }

        InfoUIElement CreateNewInfoUIElement()
        {
            GameObject el = GameObject.Instantiate(_prefab);

            el.SetActive(false);
            el.name = _prefab.name;

            el.transform.SetParent(_container);
            el.transform.localPosition = Vector3.zero;
            el.transform.rotation = Quaternion.identity;
            el.transform.localScale = Vector3.one;

            InfoUIElement uiEl = el.GetComponent<InfoUIElement>();

            return uiEl;
        }

        public void Init()
        {
            if (Prefab == null)
            {
                Logging.Log("[ElementPool] Could not init, no Prefab set!");
                return;
            }

            int maxPooledEls = _initialCount;

            for (var i = 0; i < maxPooledEls; ++i)
            {
                InfoUIElement newEl = CreateNewInfoUIElement();
                _elements.Enqueue(newEl);
            }

            State = PoolState.INIT;
        }

        public InfoUIElement GetFreeInfoUIElementFromPool()
        {
            if (Prefab == null) return null;

            if (_elements.Count == 0)
            {
                return CreateNewInfoUIElement();
            }

            return _elements.Dequeue();
        }

        public void ReturnElement(InfoUIElement el)
        {
            el.gameObject.SetActive(false);
            _elements.Enqueue(el);
        }

        public void Clear()
        {
            while (_elements.Count != 0)
            {
                InfoUIElement el = _elements.Dequeue();
#if UNITY_EDITOR
                GameObject.DestroyImmediate(el.gameObject);
#else
                GameObject.Destroy(el.gameObject);
#endif
            }
        }
    }

    public interface IInfoUIHovarable
    {
        Vector3 PositionInWorld { get; set; }
        Guid guid { get; set; }
        Guid uiAssetGuid { get; set; }
    }

    public class InfoUI : MonoBehaviour
    {
        public Canvas canvas3D;

        public LayerMask hoveredObjectsLayerMask;

        [System.NonSerialized]
        public Camera cam;

        [System.NonSerialized]
        public Action<Guid, string> OnLabelClicked_Event;

        [System.NonSerialized]
        public IWorldPrefabHolder PrefabHolder;

        [System.NonSerialized]
        public IWorldData WorldData;

        [System.NonSerialized]
        public IWispManager WispManager;

        [System.NonSerialized]
        public IRendermanService RendermanService;

        private Dictionary<Guid, InfoUIElement> _elementsInUse = new Dictionary<Guid, InfoUIElement>();
        private Dictionary<Guid, ElementPool> _pools = new Dictionary<Guid, ElementPool>();
        private RaycastHit[] _hoveredObjects = new RaycastHit[1];

        private List<IInfoUIHovarable> elementsToConsiderShowingUI = new List<IInfoUIHovarable>();

        public void UpdateUIFor(Camera cam, Vector3 mousePosition, bool showHovered, float minHoverDistance = 100.0f)
        {
            elementsToConsiderShowingUI.Clear();

            foreach (var el in _elementsInUse.Values)
            {
                el.InUse = false;
            }

            // Check if we have any objects that we hovered with the mouse
            // Currently it will check just for one (size of the _hoverObjects array is just 1)

            IInfoUIHovarable hoveredWorldObject = null;

            Ray r = cam.ScreenPointToRay(mousePosition);

            int numHovered = Physics.RaycastNonAlloc(r, _hoveredObjects, minHoverDistance, hoveredObjectsLayerMask);

            if (numHovered > 0 && showHovered)
            {
                for (var i = 0; i < _hoveredObjects.Length; ++i)
                {
                    var parentTransform = _hoveredObjects[i].transform.parent;

                    IInfoUICapable infoUIHoveredComp = parentTransform.GetComponentInParent<IInfoUICapable>();

                    if (infoUIHoveredComp == null) continue;

                    if (infoUIHoveredComp is FullWispManager)
                    {
                        if (WispManager.GetWisps().ContainsKey(infoUIHoveredComp.guid))
                        {
                            hoveredWorldObject = (IInfoUIHovarable)WispManager.GetWisps()[infoUIHoveredComp.guid];
                        }
                    }

                    // if it is a AlphaStructureDriver, it is a 3D object in the World
                    if (infoUIHoveredComp is AlphaStructureDriver)
                    {
                        hoveredWorldObject = (IInfoUIHovarable)WorldData.Get(infoUIHoveredComp.guid);
                    }

                    if (hoveredWorldObject != null && hoveredWorldObject.uiAssetGuid != Guid.Empty)
                    {
                        elementsToConsiderShowingUI.Add(hoveredWorldObject);
                    }

                }
            }

            // Process all objects that needs to show/update their UI
            for (var i = 0; i < elementsToConsiderShowingUI.Count; ++i)
            {
                float distanceSq = (cam.transform.position - elementsToConsiderShowingUI[i].PositionInWorld).sqrMagnitude;

                bool objectUIVisible = _elementsInUse.ContainsKey(elementsToConsiderShowingUI[i].guid);

                if (objectUIVisible)
                {
                    OnUpdateUI(elementsToConsiderShowingUI[i], distanceSq);
                }
                else
                {
                    OnShowUI(elementsToConsiderShowingUI[i], distanceSq).Forget();
                }
            }

            ReleaseUnusedUIElements();
        }

        bool IsOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }


        void ReleaseUnusedUIElements()
        {
            List<InfoUIElement> toRemove = new List<InfoUIElement>();

            foreach (var el in _elementsInUse.Values)
            {
                if (!el.InUse)
                {
                    toRemove.Add(el);
                }
            }

            // remove/release elements in a sparate loop,
            // because we can not iterate and remove from a dictionary at the same time
            for (var i = 0; i < toRemove.Count; ++i)
            {
                OnHideUI(toRemove[i]);
            }
        }

        public void HideAll()
        {

            foreach (var el in _elementsInUse.Values)
            {
                el.InUse = false;
            }

            ReleaseUnusedUIElements();
        }

        async UniTask OnShowUI(IInfoUIHovarable hoveredObject, float distanceSq)
        {
            if (hoveredObject.uiAssetGuid == Guid.Empty) return;

            if (!_pools.ContainsKey(hoveredObject.uiAssetGuid))
            {
                // creates a new pool
                ElementPool newPool = new ElementPool(canvas3D.transform, 3);
                _pools[hoveredObject.uiAssetGuid] = newPool;

                try
                {
                    newPool.State = PoolState.DOWNLOADING;
                    GameObject prefab = await PrefabHolder.GetAssetAsync(hoveredObject.uiAssetGuid.ToString());

                    _pools[hoveredObject.uiAssetGuid].Prefab = prefab;
                    _pools[hoveredObject.uiAssetGuid].Init();

                }
                catch (Exception ex)
                {
                    newPool.State = PoolState.ERROR;
                    Logging.Log("[InfoUI] Could not download the UI asset for: " + hoveredObject.uiAssetGuid + ". " + ex.Message);
                    return;
                }

            }
            else
            {
                if (_pools[hoveredObject.uiAssetGuid].State == PoolState.DOWNLOADING || _pools[hoveredObject.uiAssetGuid].State == PoolState.ERROR)
                {
                    return;
                }
            }


            var newUIEl = _pools[hoveredObject.uiAssetGuid].GetFreeInfoUIElementFromPool();

            if (newUIEl == null) return;

            newUIEl.InUse = true;
            newUIEl.gameObject.SetActive(true);

            if (hoveredObject is WorldObject)
            {
                WorldObject wo = (WorldObject)hoveredObject;

                foreach (var label in wo.textlabels)
                {
                    newUIEl.UpdateTextLabels(label.Key, label.Value.text);
                }

                foreach (var texture in newUIEl.Textures)
                {
                    if (!wo.textures.ContainsKey(texture.Key)) continue;

                    DownloadTexture(texture.Key, wo.textures[texture.Key].originalHash, newUIEl).Forget();

                }
            }

            // Convert world position
            Vector3 objectPosition = hoveredObject.PositionInWorld;
            Vector3 uiPosition = RectTransformUtility.WorldToScreenPoint(cam, objectPosition);

            newUIEl.transform.position = uiPosition;

            newUIEl.guid = hoveredObject.guid;
            newUIEl.assetGuid = hoveredObject.uiAssetGuid;
            newUIEl.OnLabelClicked += OnLabelClicked;
            newUIEl.SetLOD(distanceSq);
            newUIEl.name = hoveredObject.guid.ToString();

            _elementsInUse[hoveredObject.guid] = newUIEl;
        }

        void OnUpdateUI(IInfoUIHovarable hoveredObject, float distanceSq)
        {
            Vector3 objectPosition = hoveredObject.PositionInWorld;
            Vector3 uiPosition = RectTransformUtility.WorldToScreenPoint(cam, objectPosition);

            _elementsInUse[hoveredObject.guid].transform.position = uiPosition;
            _elementsInUse[hoveredObject.guid].InUse = true;
            _elementsInUse[hoveredObject.guid].SetLOD(distanceSq);
        }

        async UniTask DownloadTexture(string label, string hash, InfoUIElement element)
        {
            try
            {
                Texture t = await RendermanService.DownloadTexture(hash, RendermanTextureSize.s5);
                element.UpdateTextures(label, t);
            }
            catch
            {

            }
        }

        void OnHideUI(InfoUIElement el)
        {
            // update all textures
            foreach (var texture in el.Textures)
            {
                for (var i = 0; i < texture.Value.Count; ++i)
                {
                    texture.Value[i].ClearTexture();
                }
            }

            // Set all text labels to their default values
            foreach (var label in el.TextLabels)
            {
                for (var i = 0; i < label.Value.Count; ++i)
                {
                    label.Value[i].Text = label.Value[i].DefaultValue;
                }
            }

            el.OnLabelClicked -= OnLabelClicked;
            el.SetLOD(-1);
            _pools[el.assetGuid].ReturnElement(el);

            // remove from in use
            _elementsInUse.Remove(el.guid);
        }

        public void OnPlayerAvatarSpawned(GameObject avatar)
        {
            Camera avatarCamera = avatar.GetComponentInChildren<Camera>(true);

            if (!avatarCamera)
            {
                Logging.Log("[InfoUI] Avatar does not have a Camera");
                return;
            }

            canvas3D.worldCamera = avatarCamera;
        }

        public void OnLabelClicked(Guid guid, string label)
        {
            OnLabelClicked_Event?.Invoke(guid, label);
        }

        public void Clear()
        {
            // Return all elements to the pools
            foreach (var el in _elementsInUse.Values)
            {
                el.OnLabelClicked -= OnLabelClicked;
                _pools[el.assetGuid].ReturnElement(el);
            }

            _elementsInUse.Clear();

            // Clear and reset the pools
            foreach (var pool in _pools.Values)
            {
                pool.Clear();
            }

            _pools.Clear();
        }

        public void UpdateHoveredObjects(Camera cam, float distance = 30.0f)
        {


            // get the first hovered object

        }
    }
}
