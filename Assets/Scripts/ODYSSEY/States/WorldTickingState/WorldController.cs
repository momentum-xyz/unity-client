using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Odyssey.Networking;
using UnityEngine;

namespace Odyssey
{
    public class WorldController : StateController
    {
        public WorldController(IMomentumContext context) : base(context)
        {
        }

        public override void OnEnter()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;
        }

        public override void OnExit()
        {
            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {
            switch (msg)
            {
                case PosBusAddStaticObjectsMsg m:
                    AddStaticObjects(m.objects).Forget();
                    break;
                case PosBusRemoveStaticObjectsMsg m:
                    RemoveStaticObjects(m.objectIds);
                    break;
                case PosBusObjectDefinition m:
                    NewObjectDefinition(m.metadata).Forget();
                    break;
                case PosBusAddActiveObjectsMsg m:
                    break;
                case PosBusRemoveActiveObjectsMsg m:
                    break;
                case PosBusSetStaticObjectPositionMsg m:
                    SetStaticObjectPosition(m.objectId, m.position);
                    break;
            }
        }

        async UniTask AddStaticObjects(ObjectMetadata[] newObjects)
        {
            for (var i = 0; i < newObjects.Length; ++i)
            {
                WorldObject newWorldObject = _c.Get<IWorldDataService>().AddOrUpdateWorldObject(newObjects[i]);

                if (newWorldObject != null)
                {
                    await _c.Get<ISpawner>().SpawnWorldObject(newWorldObject, false);
                }
            }
        }

        async UniTask NewObjectDefinition(ObjectMetadata metadata)
        {
            WorldObject newWorldObject = _c.Get<IWorldDataService>().AddOrUpdateWorldObject(metadata);

            if (newWorldObject != null)
            {
                await _c.Get<ISpawner>().SpawnWorldObject(newWorldObject, false);
            }
        }

        void SetStaticObjectPosition(Guid objectId, Vector3 position)
        {
            _c.Get<IWorldDataService>().UpdatePositionForObject(objectId, position);
        }

        void RemoveStaticObjects(Guid[] objectIds)
        {
            for (var i = 0; i < objectIds.Length; ++i)
            {
                _c.Get<IWorldDataService>().RemoveWorldObject(objectIds[i]);
            }
        }

    }
}