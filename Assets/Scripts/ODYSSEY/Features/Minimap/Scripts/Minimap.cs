using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Odyssey;
using Odyssey.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    GameObject Viewport;

    [SerializeField]
    RectTransform Map;

    [SerializeField]
    Button CenterOnPlayerButton;

    [SerializeField]
    GameObject HiddenCloseButton;

    [SerializeField]
    TextMeshProUGUI ZoomLevelLabel;

    [SerializeField]
    CanvasGroup mapCanvasGroup;

    [Header("Settings")]
    [SerializeField]
    [Tooltip("How many units in world space is one in minimap space. Bigger number means more space (zoomed in), lower number means less space (zoomed out).")]
    float DefaultWorldToMinimapScale;

    [SerializeField]
    float MaxZoomIn = 10f;

    [SerializeField]
    float MaxZoomOut = 1f;

    [SerializeField]
    [Tooltip("How fast zoom in/out with mouse scroll. Bigger is faster.")]
    float ScrollDeltaToScaleChange = 0.25f;


    [Header("Prefabs")]
    [SerializeField]
    GameObject PlatformIndicatorPrefab;

    [SerializeField]
    GameObject WispIndicatorPrefab;

    [SerializeField]
    GameObject PlayerIndicatorPrefab;

    public MinimapDriver Driver { get; set; }

    void Awake()
    {
        Viewport.SetActive(false);
        _animator = GetComponent<Animator>();
        _objectIndicators = new Dictionary<string, (WorldObject, MinimapSpace)>();
        _wispIndicators = new Dictionary<string, MinimapWisp>();
    }

    void Start()
    {
        HALF_MAP_SIZE = Map.sizeDelta.x / 2;
        MAP_SIZE = Map.sizeDelta.x;

        _mapZoom = .5f;
        ZoomLevelLabel.text = String.Format("zoom {0:f}", _mapZoom);
    }

    void Update()
    {
        if (_playerAvatarTransform == null || _avatarIndicator == null) return;

        //update player indicator position
        _avatarIndicator.anchoredPosition = ConvertToMinimapSpace(_playerAvatarTransform.position);

        //rotate map around player
        Map.pivot = (new Vector2(HALF_MAP_SIZE, HALF_MAP_SIZE) + _avatarIndicator.anchoredPosition) / MAP_SIZE;
        var playerAvatarRotationY = _playerAvatarTransform.rotation.eulerAngles.y;
        var angles = Map.rotation.eulerAngles;
        angles.z = playerAvatarRotationY;
        Map.rotation = Quaternion.Euler(angles);
        Map.localScale = new Vector3(_mapZoom, _mapZoom, _mapZoom);

        // keep the world rotation of the radar to always look up
        // after the map is rotated
        // _avatarIndicatorRadar.eulerAngles = new Vector3(0, 0, 0);

        //update wisp indicator positions and rotate them counter to map rotation
        foreach (var minimapWisp in _wispIndicators.Values)
        {
            var wispData = minimapWisp.Data;
            var go = minimapWisp.gameObject;
            var rt = ((RectTransform)go.transform);
            rt.anchoredPosition = ConvertToMinimapSpace(wispData.currentPosition);
            rt.localRotation = Quaternion.Euler(0, 0, -playerAvatarRotationY);
        }

        _avatarIndicator.localRotation = Quaternion.Euler(0, 0, -playerAvatarRotationY);

        //update platforms
        foreach (var tuple in _objectIndicators.Values)
        {
            var worldData = tuple.Item1;
            var go = tuple.Item2;
            ((RectTransform)go.transform).anchoredPosition = ConvertToMinimapSpace(worldData.position);
            ((RectTransform)go.transform).localRotation = Quaternion.Euler(0, 0, -playerAvatarRotationY);
        }

        /*
        if (Input.GetKeyDown(KeyCode.M))
        {
            Viewport.SetActive(!Viewport.activeSelf);
        }
        */
    }

    void OnDestroy()
    {
        Clear();
    }

    public void ShowHide(bool show)
    {
        Viewport.gameObject.SetActive(show);
    }

    public void ToggleVisibility()
    {
        Viewport.gameObject.SetActive(!Viewport.gameObject.activeSelf);
    }

    public void Clear()
    {
        if (_objectIndicators != null)
        {
            foreach (var kvp in _objectIndicators)
            {
                Destroy(kvp.Value.Item2.gameObject);
            }
            _objectIndicators.Clear();
        }

        if (_wispIndicators != null)
        {
            foreach (var kvp in _wispIndicators)
            {
                kvp.Value.OnDoubleClicked -= this.OnMinimapWispDoubleClicked;
                kvp.Value.OnSelected -= this.OnMinimapWispSelected;
                Destroy(kvp.Value.gameObject);
            }
            _wispIndicators.Clear();
        }

        if (_avatarIndicator)
        {
            Destroy(_avatarIndicator.gameObject);
            _avatarIndicator = null;
        }

    }

    public void PlayerAvatarSpawned(GameObject playerAvatar)
    {
        _playerAvatarTransform = playerAvatar.transform;
        var indicatorGO = GameObject.Instantiate(PlayerIndicatorPrefab, Vector2.zero, Quaternion.identity, Map);
        _avatarIndicator = indicatorGO.GetComponent<RectTransform>();
    }

    public void WorldObjectSpawned(WorldObject worldObj)
    {
        if (!worldObj.showOnMiniMap) return;

        var minimapPosition = ConvertToMinimapSpace(worldObj.position);
        var indicatorGo = GameObject.Instantiate(PlatformIndicatorPrefab, Vector2.zero, Quaternion.identity, Map);

        var minimapSpace = indicatorGo.GetComponent<MinimapSpace>();
        minimapSpace.Data = worldObj;
        minimapSpace.OnSelected += OnMinimapSpaceSelected;
        minimapSpace.OnDoubleClicked += OnMinimapSpaceDoubleClicked;

        _objectIndicators.Add(worldObj.guid.ToString(), (worldObj, minimapSpace));
        ((RectTransform)indicatorGo.transform).anchoredPosition = minimapPosition;
    }

    public void WorldObjectDestroyed(WorldObject worldObj)
    {
        if (!worldObj.showOnMiniMap) return;

        string indicatorKey = worldObj.guid.ToString();
        if (!_objectIndicators.ContainsKey(indicatorKey)) return;

        var minimapSpace = _objectIndicators[indicatorKey].Item2;
        minimapSpace.OnDoubleClicked -= this.OnMinimapSpaceDoubleClicked;
        minimapSpace.OnSelected -= OnMinimapSpaceSelected;

        Destroy(_objectIndicators[indicatorKey].Item2.gameObject);
        _objectIndicators.Remove(indicatorKey);
    }

    public void WispSpawned(WispData wispData)
    {

        var minimapPosition = ConvertToMinimapSpace(wispData.currentPosition);
        var indicatorGo = GameObject.Instantiate(WispIndicatorPrefab, Vector2.zero, Quaternion.identity, Map);

        ((RectTransform)indicatorGo.transform).anchoredPosition = minimapPosition;
        var minimapWisp = indicatorGo.GetComponent<MinimapWisp>();
        minimapWisp.Data = wispData;
        minimapWisp.OnDoubleClicked += OnMinimapWispDoubleClicked;
        minimapWisp.OnSelected += OnMinimapWispSelected;

        _wispIndicators.Add(wispData.guid.ToString(), minimapWisp);

        if (_avatarIndicator) _avatarIndicator.SetAsLastSibling();
    }

    public void WispRemoved(WispData wispData)
    {
        var key = wispData.guid.ToString();
        if (!_wispIndicators.ContainsKey(key)) return;

        _wispIndicators[key].OnDoubleClicked -= this.OnMinimapWispDoubleClicked;
        _wispIndicators[key].OnSelected -= OnMinimapWispSelected;
        Destroy(_wispIndicators[key].gameObject);
        _wispIndicators.Remove(key);
    }

    public bool IsExpanded()
    {
        return _animator.GetBool("Expanded");
    }

    #region Event Handlers
    public void OnViewPortClicked()
    {
        var expanded = _animator.GetBool("Expanded");
        if (!expanded)
        {
            _animator.SetBool("Expanded", true);
            HiddenCloseButton.SetActive(true);
            mapCanvasGroup.interactable = true;
            mapCanvasGroup.blocksRaycasts = true;
            _mapZoom = 1f;
        }
    }

    public void OnCloseButtonClicked()
    {
        _animator.SetBool("Expanded", false);
        OnCenterOnPlayerButtonClicked();
        HiddenCloseButton.SetActive(false);
        _mapZoom = DefaultWorldToMinimapScale;
        mapCanvasGroup.interactable = false;
        mapCanvasGroup.blocksRaycasts = false;
        _mapZoom = .5f;
        SetIndicatorsScale(1f);
    }

    public void ToggleExpand()
    {
        var expanded = _animator.GetBool("Expanded");
        if (expanded) OnCloseButtonClicked();
        else OnViewPortClicked();
    }

    public void OnCenterOnPlayerButtonClicked()
    {
        Map.anchoredPosition = Vector2.zero;
        CenterOnPlayerButton.gameObject.SetActive(false);
    }

    public void OnBeginDrag(BaseEventData eventData)
    {
    }

    public void OnDrag(BaseEventData eventData)
    {
        if (!_animator.GetBool("Expanded")) return;

        var e = (PointerEventData)eventData;
        Map.anchoredPosition += e.delta;
        CenterOnPlayerButton.gameObject.SetActive(true);
    }

    public void OnEndDrag(BaseEventData eventData)
    {
    }

    public void OnScroll(BaseEventData eventData)
    {
        if (!_animator.GetBool("Expanded")) return;
        var e = (PointerEventData)eventData;

        float zoomDelta = e.scrollDelta.y * ScrollDeltaToScaleChange;
        var newZoom = Mathf.Clamp(_mapZoom + zoomDelta, MaxZoomOut, MaxZoomIn);
        bool zoomChanged = newZoom != _mapZoom;
        _mapZoom = newZoom;
        ZoomLevelLabel.text = String.Format("zoom {0:f}", _mapZoom);

        Vector2 cursorPosInsideMap;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Map, e.position, e.pressEventCamera, out cursorPosInsideMap);
        Vector2 panDelta = Map.localRotation * ((Vector3)cursorPosInsideMap - _avatarIndicator.localPosition);
        if (zoomChanged)
        {
            Map.anchoredPosition -= panDelta * zoomDelta;

            float scale = 1f;
            if (_mapZoom < 1)
                scale += 1 - _mapZoom;
            SetIndicatorsScale(scale);
        }
    }

    #endregion

    Vector2 ConvertToMinimapSpace(Vector3 worldPos)
    {
        return new Vector2(worldPos.x * DefaultWorldToMinimapScale, worldPos.z * DefaultWorldToMinimapScale);
    }

    void OnMinimapWispDoubleClicked(MinimapWisp minimapWisp)
    {
        Driver.TeleportToUser(minimapWisp.Data.guid.ToString());
        OnCloseButtonClicked();
    }

    void OnMinimapWispSelected(MinimapWisp minimapWisp)
    {
        minimapWisp.SetPlayerName(minimapWisp.Data.name);

        //keep the last clicked element at the top, but keep the player on top of it also
        minimapWisp.transform.SetAsLastSibling();
        if (_avatarIndicator) _avatarIndicator.SetAsLastSibling();
    }

    void OnMinimapSpaceDoubleClicked(MinimapSpace minimapSpace)
    {
        Driver.TeleportToSpace(minimapSpace.Data.guid.ToString());
        OnCloseButtonClicked();
    }

    void OnMinimapSpaceSelected(MinimapSpace minimapSpace)
    {
        minimapSpace.SetSpaceName(minimapSpace.Data.name);

        //keep the last clicked element at the top, but keep the player on top of it also
        minimapSpace.transform.SetAsLastSibling();
        if (_avatarIndicator) _avatarIndicator.SetAsLastSibling();
    }

    void SetIndicatorsScale(float scale)
    {
        var scaleVector = new Vector3(scale, scale, scale);
        foreach (var minimapWisp in _wispIndicators.Values)
        {
            var go = minimapWisp.gameObject;
            go.transform.localScale = scaleVector;
        }
        foreach (var tuple in _objectIndicators.Values)
        {
            var go = tuple.Item2.gameObject;
            go.transform.localScale = scaleVector;
        }
        _avatarIndicator.localScale = scaleVector;
    }

    Animator _animator;
    Transform _playerAvatarTransform;
    Dictionary<string, (WorldObject, MinimapSpace)> _objectIndicators;
    Dictionary<string, MinimapWisp> _wispIndicators;
    RectTransform _avatarIndicator;
    float _mapZoom;

    float HALF_MAP_SIZE;
    float MAP_SIZE;
}
