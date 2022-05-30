using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Odyssey
{
    public class LookAtWispController : StateController
    {
        public LookAtWispController(IMomentumContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            _c.Get<IReactBridge>().LookAtWisp_Event += OnLookAtWisp;
        }

        public override void OnExit()
        {
            _c.Get<IReactBridge>().LookAtWisp_Event -= OnLookAtWisp;
        }


        void OnLookAtWisp(string wispGuid)
        {
            Guid wispID = Guid.Parse(wispGuid);
            Vector3 wispPosition = _c.Get<IWispManager>().GetWispPosition(wispID);
            LookAtDestination(_c.Get<ISessionData>().WorldAvatarController.transform, wispPosition, 1.5f).Forget();
        }

        async UniTask LookAtDestination(Transform transform, Vector3 destination, float timeToLookAt)
        {
            var currentRotation = transform.rotation;
            float time = 0f;

            var targetRotation = Quaternion.LookRotation(destination - transform.position);

            while (time < 1f)
            {
                time += Time.deltaTime / timeToLookAt;
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, time);
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}