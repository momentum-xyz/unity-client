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
                        _c.Get<IStateMachine>().SwitchState(typeof(SpawnWorldState));
                    }
                    break;
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