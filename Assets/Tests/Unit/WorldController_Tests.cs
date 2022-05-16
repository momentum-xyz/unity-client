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
    public class WorldController_Tests
    {
        [SetUp]
        public void Init()
        {
            _worldDataServiceMock = Substitute.For<IWorldDataService>();
            _sessionDataMock = Substitute.For<ISessionData>();
            _stateMachineMock = Substitute.For<IStateMachine>();
            _posBusMock = Substitute.For<IPosBus>();

            var context = new MomentumContext();
            context.RegisterService(_worldDataServiceMock);
            context.RegisterService(_sessionDataMock);
            context.RegisterService(_stateMachineMock);
            context.RegisterService(_posBusMock);

            _controller = new WorldController(context);
        }

        [TearDown]
        public void Cleanup()
        { /* ... */ }

        [Test]
        public void ShouldUpdateStaticObjectPosition()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            Vector3 position = new Vector3(99, 99, 99);

            _controller.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusSetStaticObjectPositionMsg()
            {
                position = position,
                objectId = guid
            });

            _worldDataServiceMock.Received().UpdatePositionForObject(guid, position);

        }

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

            _controller.OnEnter();
            _posBusMock.OnPosBusMessage(new PosBusAddStaticObjectsMsg()
            {
                objects = new ObjectMetadata[] { metadata }
            });

            _worldDataServiceMock.Received().AddWorldObject(metadata);
        }

        [Test]
        public void ShouldRemoveStaticObject()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");

            _controller.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusRemoveStaticObjectsMsg()
            {
                objectIds = new Guid[] { guid }
            });

            _worldDataServiceMock.Received().RemoveWorldObject(guid);
        }


        IWorldDataService _worldDataServiceMock;
        ISessionData _sessionDataMock;
        IStateMachine _stateMachineMock;
        IPosBus _posBusMock;
        WorldController _controller;
    }
}