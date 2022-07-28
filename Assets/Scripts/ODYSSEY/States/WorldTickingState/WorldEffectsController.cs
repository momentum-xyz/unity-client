using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;
using System;
using Cysharp.Threading.Tasks;

namespace Odyssey
{
    public class WorldEffectsController : StateController
    {
        private IMomentumAPI _api;
        public WorldEffectsController(IMomentumContext context) : base(context)
        {
            _api = context.Get<IMomentumAPI>();
        }

        public override void OnEnter()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;
        }

        public override void OnExit()
        {
            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        async UniTask TriggerEffect(Guid effectEmitter, WorldObject positionObject, int effectType)
        {
            if (!await WaitUnitilObjectIsSpawned(effectEmitter)) return;
            _api.PublishEffect(effectEmitter, positionObject.WorldPosition(), positionObject.GO, effectType);

        }


        async UniTask TriggerEffect(Guid effectEmitter, Vector3 position, int effectType)
        {
            if (!await WaitUnitilObjectIsSpawned(effectEmitter)) return;
            _api.PublishEffect(effectEmitter, position, null, effectType);
        }

        async UniTask TriggerBridgeEffect(Guid effectEmitter, WorldObject source, WorldObject dest, int effectType)
        {
            if (!await WaitUnitilObjectIsSpawned(effectEmitter)) return;
            _api.PublishBridgeEffect(effectEmitter, source.WorldPosition(), dest.WorldPosition(), source.GO, dest.GO, effectType);

        }

        async UniTask TriggerBridgeEffect(Guid effectEmitter, Vector3 source, Vector3 dest, int effectType)
        {
            if (!await WaitUnitilObjectIsSpawned(effectEmitter)) return;

            _api.PublishBridgeEffect(effectEmitter, source, dest, null, null, effectType);

        }

        /// <summary>
        /// Sometimes we may receive an Effects PosBus message for an object that has his asset still being download
        /// and in order to handle that case correctly, we will wait until the object is spawned.. and then trigger the effect
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>

        async UniTask<bool> WaitUnitilObjectIsSpawned(Guid objectId)
        {
            WorldObject wo = _c.Get<IWorldData>().Get(objectId);

            if (wo == null)
            {
                Logging.LogError("[WorldEffectsController] The emitter object with GUID: " + objectId + " is not found..");
                return false;
            }

            if (wo.state == WorldObjectState.DOWNLOADING_ASSET)
            {

                await UniTask.WaitUntil(() =>
                {
                    return wo.state != WorldObjectState.DOWNLOADING_ASSET;
                });

                if (wo.state != WorldObjectState.SPAWNED) return false;
            }

            return true;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {

            switch (msg)
            {
                case PosBusTransitionalEffectOnPositionMsg m:
                    TriggerEffect(m.Emmiter, m.Position, (int)m.Effect).Forget();
                    break;
                case PosBusTransitionalEffectOnObjectMsg m:

                    var wo = _c.Get<IWorldData>().Get(m.Object);
                    if (wo == null) return;

                    TriggerEffect(m.Emmiter, wo, (int)m.Effect).Forget();

                    break;
                case PosBusTransitionalBridgingEffectOnObjectMsg m:

                    var source = _c.Get<IWorldData>().Get(m.ObjectFrom);
                    var dest = _c.Get<IWorldData>().Get(m.ObjectTo);

                    if (source == null || dest == null)
                    {
                        string notFoundErrMsg = "";
                        if (source == null) notFoundErrMsg += "Source: " + m.ObjectFrom.ToString() + " not found..";
                        if (dest == null) notFoundErrMsg += "Dest: " + m.ObjectTo.ToString() + " not found..";
                        Logging.Log("[WorldEffectController] " + notFoundErrMsg + " Emitter: " + m.Emmiter.ToString() + " effectID: " + m.Effect);
                        return;
                    }

                    TriggerBridgeEffect(m.Emmiter, source, dest, (int)m.Effect).Forget();

                    break;
                case PosBusTransitionalBridgingEffectOnPositionMsg m:
                    TriggerBridgeEffect(m.Emmiter, m.PositionFrom, m.PositionTo, (int)m.Effect).Forget();
                    break;
            }
        }



    }
}
