using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public interface IInfoUIDriver
    {
        public Action<Guid, string> OnLabelClicked_Event { get; set; }
        public void SetCamera(Camera cam);
        public void UpdateDriver(Vector3 mousePosition, bool showHovered = true);
        public void HideAll();
        public void Clear();
    }

    public class InfoUIDriver : MonoBehaviour, IRequiresContext, IInfoUIDriver
    {
        public Action<Guid, string> OnLabelClicked_Event { get; set; }

        public float activateInfoUIOnDistanceSq = 40.0f * 40.0f;
        public float canHoverAtDistance = 100.0f;
        public InfoUI infoUI;

        IMomentumContext _c;
        ILODSystem _lodSystem;
        ISessionData _sessionData;

        public void Init(IMomentumContext context)
        {
            _c = context;
        }

        void OnEnable()
        {
            _c.Get<ISpawner>().OnPlayerAvatarSpawned += OnPlayerAvatarSpawned;
            infoUI.OnLabelClicked_Event += OnLabelClicked;
        }

        void OnDisable()
        {
            _c.Get<ISpawner>().OnPlayerAvatarSpawned -= OnPlayerAvatarSpawned;
            infoUI.OnLabelClicked_Event -= OnLabelClicked;
        }

        // Start is called before the first frame update
        void Start()
        {
            _lodSystem = _c.Get<ILODSystem>();
            _sessionData = _c.Get<ISessionData>();
            infoUI.PrefabHolder = _c.Get<IWorldPrefabHolder>();
            infoUI.WorldData = _c.Get<IWorldData>();
            infoUI.RendermanService = _c.Get<IRendermanService>();
            infoUI.WispManager = _c.Get<IWispManager>();

        }

        public void SetCamera(Camera cam)
        {
            infoUI.cam = cam;
        }

        // Update is called once per frame
        public void UpdateDriver(Vector3 mousePosition, bool showHovered = true)
        {
            infoUI.UpdateUIFor(_sessionData.AvatarCamera, mousePosition, showHovered, canHoverAtDistance);

        }

        void OnPlayerAvatarSpawned(GameObject avatar)
        {
            infoUI.OnPlayerAvatarSpawned(avatar);
        }

        void OnLabelClicked(Guid guid, string label)
        {
            OnLabelClicked_Event?.Invoke(guid, label);
        }

        public void Clear()
        {
            infoUI.Clear();
        }

        public void HideAll()
        {
            infoUI.HideAll();
        }
    }

}
