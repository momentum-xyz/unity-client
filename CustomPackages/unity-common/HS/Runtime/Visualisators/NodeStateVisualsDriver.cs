using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class NodeStateVisualsDriver : MonoBehaviour
    {
        [SerializeField] [ColorUsage(true, true)] Color _inactiveColor;
        [SerializeField] [ColorUsage(true, true)] Color _paraChainColor;
        [SerializeField] [ColorUsage(true, true)] Color _relayChainColor;

        [SerializeField] Vector2 _healthScales = new Vector2(0.8f, 1.2f);
        [SerializeField] Transform _healthObject;
        [SerializeField] GameObject _selectedNotif;

        [Header("Storage")]
        [SerializeField] List<Renderer> _renderers;

        [Header("Params")]
        [SerializeField] [Range(0, 1)] float _health;

        [Header("State")]
        [SerializeField] bool _isClaimed;
        [SerializeField] bool _isActive;
        [SerializeField] bool _isPara;
        [SerializeField] bool _isOnline;
        [SerializeField] bool _isSelected;

        Vector3 _baseHealthScale = Vector3.one;
        MaterialPropertyBlock _props;

        const string _TINTPROP = "_StatefulTint";


        public void SetState(bool isClaimed, bool isActive, bool isPara, bool isOnline, bool isSelected)
        {
            _isClaimed = isClaimed;
            _isActive = isActive;
            _isPara = isPara;
            _isOnline = isOnline;
            _isSelected = isSelected;
            UpdateStateVisuals();
        }

        public void SetClaimed(bool isClaimed)
        {
            _isClaimed = isClaimed;
            UpdateStateVisuals();
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            UpdateStateVisuals();
        }

        public void SetPara(bool isPara)
        {
            _isPara = isPara;
            UpdateStateVisuals();
        }

        public void SetOnline(bool isOnline)
        {
            _isOnline = isOnline;
            UpdateStateVisuals();
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            UpdateStateVisuals();
        }


        public void SetParameters(float normalizedHealth)
        {
            _health = normalizedHealth;
            _healthObject.localScale = _baseHealthScale * Mathf.Lerp(_healthScales.x, _healthScales.y, _health);
        }


        void OnValidate()
        {
            if (Application.isPlaying && _didInit) SetParameters(_health); // hasGathered prevents Validate from running before Awake()
            UpdateStateVisuals();
        }


        bool _didInit = false;
        void Awake()
        {
            _baseHealthScale = _healthObject.localScale;
            _didInit = true;
        }


        void UpdateStateVisuals()
        {
            if (_props == null) _props = new MaterialPropertyBlock();
            _props.SetColor(
                _TINTPROP,
                _isActive
                    ? _isPara
                        ? _paraChainColor
                        : _relayChainColor
                    : _inactiveColor
            );
            foreach (var r in _renderers) r.SetPropertyBlock(_props);
            if (Application.isPlaying)
            {
                _healthObject.gameObject.SetActive(_isActive);
                _selectedNotif.SetActive(_isSelected);
            }
        }

        [ContextMenu("Gather Renderers")]
        void EditorGatherRenderers()
        {
            foreach (var r in GetComponentsInChildren<Renderer>(true))
                foreach (var m in r.sharedMaterials)
                    if (m.HasProperty(_TINTPROP))
                    {
                        _renderers.Add(r);
                        break;
                    }
        }
    }
}