using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using System;
using HS;

namespace Odyssey
{

    public interface IFollowUserController
    {
        public bool IsFollowing { get; set; }
        public void Follow(Guid userGuid);
        public void StopFollowing();

        public void Update(float deltaTime);
    }

    public class FollowUserController : IRequiresContext, IFollowUserController
    {
        public bool IsFollowing { get => _isFollowing; set { _isFollowing = value; } }

        IMomentumContext _c;
        bool _isFollowing = false;

        WispData _followingWisp;
        IThirdPersonController _thirdPersonController;

        public void Follow(Guid userGuid)
        {
            _c.Get<IWispManager>().OnWispRemoved += OnWispRemoved;

            if (!_c.Get<IWispManager>().GetWisps().ContainsKey(userGuid))
            {
                Logging.Log("[FollowUserController] Can not follow user: " + userGuid + ", not found!");
                return;
            }

            _followingWisp = _c.Get<IWispManager>().GetWisps()[userGuid];
            _thirdPersonController = _c.Get<IThirdPersonController>();

            _thirdPersonController.CanMove = false;
            _isFollowing = true;
        }

        public void StopFollowing()
        {
            _c.Get<HS.IThirdPersonController>().CanMove = true;
            _isFollowing = false;

            _c.Get<IWispManager>().OnWispRemoved -= OnWispRemoved;
        }

        void OnWispRemoved(WispData d)
        {
            if (!_isFollowing || _followingWisp == null) return;

            if (d.guid == _followingWisp.guid)
            {
                Logging.Log("[FollowUserController] Leader disconnected...stopping follow mode.");
                StopFollowing();
            }
        }

        public void Init(IMomentumContext context)
        {
            _c = context;
        }

        public void Update(float deltaTime)
        {
            if (!_isFollowing || _followingWisp == null) return;

            Vector3 dir = (_followingWisp.currentPosition - _thirdPersonController.CurrentPosition).normalized;

            // Lerp position to the Leader
            Vector3 targetPosition = _followingWisp.currentPosition - dir * 5.0f;
            Vector3 currentPosition = _thirdPersonController.CurrentPosition;
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, deltaTime);
            _thirdPersonController.CurrentPosition = newPosition;

            // Lerp rotation, only if the user is not currently looking around on it's own

            if (!_thirdPersonController.IsRotating)
            {
                Quaternion currentRotation = _thirdPersonController.CurrentRotation;
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

                Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, deltaTime);
                _thirdPersonController.CurrentRotation = newRotation;
            }
        }

        void OnDrawGizmos()
        {

        }

    }

}