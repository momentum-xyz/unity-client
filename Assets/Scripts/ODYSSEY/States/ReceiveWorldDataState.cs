using Cysharp.Threading.Tasks;
using Odyssey.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public class ReceiveWorldDataState : IState
    {
        IMomentumContext _c;
        IWorldDataService _worldDataService;
        StateController[] _controllers;

        public ReceiveWorldDataState(IMomentumContext context)
        {
            _c = context;
            _worldDataService = _c.Get<IWorldDataService>();
            _controllers = new StateController[] {
                new WorldObjectsMetadataController(context)
            };
        }

        public void OnEnter()
        {
            _c.Get<IPosBus>().OnPosBusMessage += OnPosBusMessage;

            foreach (var controller in _controllers)
            {
                controller.OnEnter();
            }

        }

        public void OnExit()
        {
            foreach (var controller in _controllers)
            {
                controller.OnExit();
            }

            _c.Get<IPosBus>().OnPosBusMessage -= OnPosBusMessage;
        }

        void OnPosBusMessage(IPosBusMessage msg)
        {
            switch (msg)
            {
                case PosBusObjectDefinition m:

                    ObjectMetadata metaData = m.metadata;

                    _worldDataService.AddOrUpdateWorldObject(metaData);

                    if (metaData.infoUIType != Guid.Empty)
                    {
                        _c.Get<IWorldPrefabHolder>().AddForPreload(metaData.infoUIType);
                    }

                    break;

                case PosBusSignalMsg m:

                    _c.Get<IReactAPI>().SendLoadingProgress(10);

                    if (m.signal == PosBusSignalType.Spawn)
                    {
                        _c.Get<IPosBus>().ProcessMessageQueue = false;
#if UNITY_WEBGL && !UNITY_EDITOR
                        _c.Get<ISessionData>().NetworkingConfig.injectAssets = false;
#endif
                        if (_c.Get<ISessionData>().NetworkingConfig.injectAssets)
                        {
                            InjectAssets();
                        }

                        _c.Get<IStateMachine>().SwitchState(typeof(SpawnWorldState));
                    }
                    break;
            }
        }

        private void InjectAssets()
        {
            if (_c.Get<ISessionData>().NetworkingConfig.assetsToInjectData == null) return;

            List<InjectedAsset> assets = _c.Get<ISessionData>().NetworkingConfig.assetsToInjectData.assetsToInject;

            for (var i = 0; i < assets.Count; ++i)
            {
                var a = assets[i];

                // Add the asset
                Guid assetTypeGuid = Guid.NewGuid();
                AddressableAsset addr = new AddressableAsset("", "", "");
                addr.gameObject = a.prefab;
                addr.status = AddressableAssetStatus.Loaded;

                _c.Get<IAddressablesProvider>().AddressablesAssets.Add(assetTypeGuid.ToString(), addr);

                ObjectMetadata objectMetadata = new ObjectMetadata();

                objectMetadata.objectId = Guid.Parse(a.GUID);
                objectMetadata.parentId = Guid.Parse(_c.Get<ISessionData>().WorldID);
                objectMetadata.position = a.position;
                objectMetadata.assetType = assetTypeGuid;
                objectMetadata.infoUIType = Guid.Empty;
                objectMetadata.isMinimap = false;
                objectMetadata.name = a.name;

                _worldDataService.AddOrUpdateWorldObject(objectMetadata);

                WorldObject wo = _c.Get<IWorldData>().Get(objectMetadata.objectId);

                for (var j = 0; j < a.textureData.Count; ++j)
                {
                    _worldDataService.UpdateObjectTexture(wo, a.textureData[j].label, a.textureData[j].hash);
                }


            }
        }

        public void Update()
        {

            foreach (var controller in _controllers)
            {
                controller.Update();
            }

        }


    }
}