using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSubstitute;
using Odyssey;
using Odyssey.Networking;
using System;
using HS;

namespace UnitTests
{
    [TestFixture]
    public class WorldEffectsController_Test
    {
        [SetUp]
        public void Init()
        {

            _sessionDataMock = Substitute.For<ISessionData>();
            _stateMachineMock = Substitute.For<IStateMachine>();
            _posBusMock = Substitute.For<IPosBus>();
            _worldData = Substitute.For<IWorldData>();


            var context = new MomentumContext();

            context.RegisterService(_sessionDataMock);
            context.RegisterService(_stateMachineMock);
            context.RegisterService(_posBusMock);
            context.RegisterService(_worldData);

            _controller = new WorldEffectsController(context);

            _emitterGuid = Guid.NewGuid();
            _sourceGuid = Guid.NewGuid();
            _destinationGuid = Guid.NewGuid();

            _emitterObject = new WorldObject();
            _emitterObject.position = Vector3.zero;
            _emitterObject.guid = _emitterGuid;
            _emitterObject.state = WorldObjectState.SPAWNED;

            _sourceObject = new WorldObject();
            _sourceObject.position = new Vector3(100, 100, 100);
            _sourceObject.guid = _sourceGuid;
            _sourceObject.state = WorldObjectState.SPAWNED;
            _sourceObject.GO = new GameObject();
            _sourceObject.GO.name = "Source Object";

            _destinationObject = new WorldObject();
            _destinationObject.position = new Vector3(200, 200, 200);
            _destinationObject.guid = _destinationGuid;
            _destinationObject.state = WorldObjectState.SPAWNED;
            _destinationObject.GO = new GameObject();
            _destinationObject.GO.name = "Destination Object";

            _worldData.Get(_emitterGuid).Returns(_emitterObject);
            _worldData.Get(_sourceGuid).Returns(_sourceObject);
            _worldData.Get(_destinationGuid).Returns(_destinationObject);



        }


        [TearDown]
        public void Cleanup()
        { /* ... */ }

        [Test]
        public void ShouldTriggerEffectOnPosition()
        {
            Vector3 position = Vector3.one;
            uint effectId = 999;

            var posBusMsg = new PosBusTransitionalEffectOnPositionMsg(_emitterGuid, position, effectId);

            _controller.OnEnter();
            _posBusMock.OnPosBusMessage(posBusMsg);



        }

        [Test]
        public void ShouldTriggerEffectOnObject()
        {
            Vector3 position = Vector3.one;
            uint effectId = 999;

            var posBusMsg = new PosBusTransitionalEffectOnObjectMsg(_emitterGuid, _sourceGuid, effectId);
            _controller.OnEnter();
            _posBusMock.OnPosBusMessage(posBusMsg);

        }

        [Test]
        public void ShouldTriggerBridgeEffectOnPosition()
        {

            Vector3 sourcePosition = new Vector3(100, 100, 100);
            Vector3 destPosition = new Vector3(200, 200, 200);
            uint effectId = 999;

            var posBusMsg = new PosBusTransitionalBridgingEffectOnPositionMsg(_emitterGuid, sourcePosition, destPosition, effectId);

            _controller.OnEnter();
            _posBusMock.OnPosBusMessage(posBusMsg);



        }

        [Test]
        public void ShouldTriggerBridgeEffectWithObjects()
        {
            uint effectId = 999;
            var posBusMsg = new PosBusTransitionalBridgingEffectOnObjectMsg(_emitterGuid, _sourceGuid, _destinationGuid, effectId);
            _controller.OnEnter();
            _posBusMock.OnPosBusMessage(posBusMsg);


        }

        IWorldData _worldData;

        ISessionData _sessionDataMock;
        IStateMachine _stateMachineMock;
        IPosBus _posBusMock;
        WorldEffectsController _controller;
        WorldObject _emitterObject, _sourceObject, _destinationObject;
        Guid _emitterGuid, _sourceGuid, _destinationGuid;

    }
}