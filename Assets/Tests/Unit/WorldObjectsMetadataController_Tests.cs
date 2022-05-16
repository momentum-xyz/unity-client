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
    public class WorldObjectsMetadataController_Test
    {
        [SetUp]
        public void Init()
        {
            _networkingServiceMock = Substitute.For<INetworkingService>();
            _sessionDataMock = Substitute.For<ISessionData>();
            _worldDataServiceMock = Substitute.For<IWorldDataService>();
            _stateMachineMock = Substitute.For<IStateMachine>();
            _worldDataMock = Substitute.For<IWorldData>();
            _posBusMock = Substitute.For<IPosBus>();
            _worldObjectsStateManagerMock = Substitute.For<IWorldObjectsStateManager>();

            var context = new MomentumContext();
            context.RegisterService(_networkingServiceMock);
            context.RegisterService(_sessionDataMock);
            context.RegisterService(_worldDataServiceMock);
            context.RegisterService(_stateMachineMock);
            context.RegisterService(_worldDataMock);
            context.RegisterService(_posBusMock);
            context.RegisterService(_worldObjectsStateManagerMock);

            _controller = new WorldObjectsMetadataController(context);

        }

        [TearDown]
        public void Cleanup()
        { /* ... */ }

        [Test]
        public void ShouldUpdatePrivacyForObject()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            int privacy = 2;

            _controller.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusSetAttributesMsg()
            {
                spaceID = guid,
                attributes = new AttributeMetadata[] {new AttributeMetadata()
                {
                    attribute = privacy,
                    label = "privacy"
                } }
            });

            _worldDataServiceMock.Received().UpdatePermissionsForObject(guid, privacy);
        }

        [Test]
        public void ShouldUpdateStateForObject()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            int stagemode = 1;

            _controller.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusSetAttributesMsg()
            {
                spaceID = guid,
                attributes = new AttributeMetadata[] {new AttributeMetadata()
                {
                    attribute = stagemode,
                    label = "stagemode"
                } }
            });

            _worldObjectsStateManagerMock.Received().SetState<int>(guid.ToString(), "stagemode", stagemode);
        }

        [Test]
        public void ShouldUpdateTextures()
        {
            Guid guid = Guid.Parse("1e8569ec-9f48-4acd-8eef-31ba1d14c20d");
            string textureHash = "hash";
            string textureLabel = "label";
            TextureMetadata[] textures = new TextureMetadata[] {new TextureMetadata()
            {
                data = textureHash,
                label = textureLabel
            }};

            _controller.OnEnter();

            _posBusMock.OnPosBusMessage(new PosBusSetTexturesMsg()
            {

                objectID = guid,
                textures = textures
            });

            _worldDataServiceMock.Received().UpdateObjectTextures(guid, textures);
        }


        WorldObjectsMetadataController _controller;
        INetworkingService _networkingServiceMock;
        ISessionData _sessionDataMock;
        IWorldDataService _worldDataServiceMock;
        IStateMachine _stateMachineMock;
        IWorldData _worldDataMock;
        IPosBus _posBusMock;
        IWorldObjectsStateManager _worldObjectsStateManagerMock;

    }
};