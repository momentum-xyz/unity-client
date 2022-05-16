using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;

namespace Odyssey
{
    public class UserPositionController : StateController
    {
        AlphaUserPositionUpdater _positionUpdater;
        ISessionData _sessionData;
        IPosBus _posBus;

        public UserPositionController(IMomentumContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;

            _sessionData = _c.Get<ISessionData>();
            _posBus = _c.Get<IPosBus>();

            SetupAvatar();
        }

        public override void OnExit()
        {
            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {

        }

        public override void Update()
        {
            // Check if we have received a position update msg from PosBus and move the Avatar
            if (_sessionData.WorldAvatarController != null && _sessionData.GotSelfPositionMsg)
            {
                Debug.Log("Got Self Position Msg: " + _sessionData.SelfPosition);
                _sessionData.GotSelfPositionMsg = false;
                _sessionData.WorldAvatarController.transform.position = _sessionData.SelfPosition;
            }

        }

        public void SetupAvatar()
        {

            if (_c.Get<ISessionData>().WorldAvatarController == null)
            {
                Logging.LogError("Avatar is not setup!");
                return;
            }

            // set the Wisps to use the default material
            _c.Get<IWispManager>().SetDefaultParticleMaterial();

            // Apply AvatarOptions if there are any
            AvatarOptions options = _c.Get<ISessionData>().WorldAvatarController.GetComponent<AvatarOptions>();
            if (options != null)
            {
                if (options.updateMaterial)
                {
                    _c.Get<IWispManager>().SetWispsParticlesMaterial(options.newMaterial);
                }

                if (options.updateSwitchToParticleDistance)
                {
                    _c.Get<IWispManager>().SetFullWispSwitchDistance(options.newDistance);
                }

                if (options.FullWispPrefab != null)
                {
                    _c.Get<IWispManager>().SetFullWispPrefab(options.FullWispPrefab);
                }
            }


            // Attach the Avatar position updater to PosBus

            _positionUpdater = _c.Get<ISessionData>().WorldAvatarController.GetComponent<AlphaUserPositionUpdater>();

            if (_positionUpdater == null)
            {
                _positionUpdater = _c.Get<ISessionData>().WorldAvatarController.AddComponent<AlphaUserPositionUpdater>();
            }

            if (_positionUpdater != null)
            {
                _positionUpdater.PositionUpdated_Event += SendPositionToPosBus;
                _positionUpdater.StartPositionUpdates();
            }

            // Setup all services that needs the Avatar Camera
            if (_c.Get<ISessionData>().AvatarCamera != null)
            {
                _c.Get<ILODSystem>().RunLOD(_c.Get<ISessionData>().AvatarCamera.transform.position);
                _c.Get<IInfoUIDriver>().SetCamera(_c.Get<ISessionData>().AvatarCamera);
            }

        }

        void SendPositionToPosBus(Vector3 position)
        {
            if (_sessionData.AppPaused) return;
            _posBus.SendPosition(position);
        }

    }

}