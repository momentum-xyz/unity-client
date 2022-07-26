using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;
using Odyssey;
using Odyssey.Networking;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class ReceiveWorldDataState_Tests
    {
        [SetUp]
        public void Init()
        {

            _sessionDataMock = Substitute.For<ISessionData>();
            _worldDataServiceMock = Substitute.For<IWorldDataService>();
            _stateMachineMock = Substitute.For<IStateMachine>();
            _worldDataMock = Substitute.For<IWorldData>();
            _posBusMock = Substitute.For<IPosBus>();
            _worldObjectsStateManagerMock = Substitute.For<IWorldObjectsStateManager>();

            var context = new MomentumContext();

            context.RegisterService(_sessionDataMock);
            context.RegisterService(_worldDataServiceMock);
            context.RegisterService(_stateMachineMock);
            context.RegisterService(_worldDataMock);
            context.RegisterService(_posBusMock);
            context.RegisterService(_worldObjectsStateManagerMock);

            _state = new ReceiveWorldDataState(context);
        }

        [TearDown]
        public void Cleanup()
        { /* ... */ }




        [Test]
        public void ShouldAddStaticObject()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            Guid parentGuid = Guid.Parse("0e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            Guid assetType = Guid.Parse("998569ec-9f48-4acd-8eef-31ba1d14c20d");
            Vector3 position = new Vector3(99, 99, 99);

            ObjectMetadata metadata = new ObjectMetadata()
            {
                name = "Test Object",
                objectId = guid,
                parentId = parentGuid,
                assetType = assetType,
                position = position
            };

            _state.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusAddStaticObjectsMsg()
            {
                objects = new ObjectMetadata[] { metadata }
            });

            _worldDataServiceMock.Received().AddOrUpdateWorldObject(metadata);
        }



        [Test]
        public void ShouldChangeToSpawnWorldStateWhenAllDataHaveBeenReceived()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            Guid parentGuid = Guid.Parse("0e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            Guid assetType = Guid.Parse("998569ec-9f48-4acd-8eef-31ba1d14c20d");
            Vector3 position = new Vector3(99, 99, 99);

            ObjectMetadata metadata = new ObjectMetadata()
            {
                name = "Test Object",
                objectId = guid,
                parentId = parentGuid,
                assetType = assetType,
                position = position
            };

            _state.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusAddStaticObjectsMsg()
            {
                objects = new ObjectMetadata[] { metadata }
            });

            _stateMachineMock.Received().SwitchState(typeof(SpawnWorldState));
        }

        [Test]
        public void ShouldHandleOnSelfPositionReceived()
        {
            _state.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusSelfPosMsg()
            {
                position = Vector3.zero
            });

            _sessionDataMock.Received().GotSelfPositionMsg = true;
            _sessionDataMock.Received().SelfPosition = Vector3.zero;
        }


        ReceiveWorldDataState _state;

        ISessionData _sessionDataMock;
        IWorldDataService _worldDataServiceMock;
        IStateMachine _stateMachineMock;
        IWorldData _worldDataMock;
        IPosBus _posBusMock;
        IWorldObjectsStateManager _worldObjectsStateManagerMock;
    }
};