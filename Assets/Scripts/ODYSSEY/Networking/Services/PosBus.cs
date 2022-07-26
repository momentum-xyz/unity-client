using System.Collections.Generic;
using UnityEngine;
using HybridWebSocket;
using System;
using FlatBuffers;
using API;
using System.Collections.Concurrent;

namespace Odyssey.Networking
{
    public interface IPosBus
    {
        public bool TokenIsNotValid { get; set; }
        public bool HasReconnected { get; set; }
        public IWebsocketsHandler WebsocketHandler { get; set; }
        public Action<IPosBusMessage> OnPosBusMessage { get; set; }
        public Action OnPosBusConnected { get; set; }
        public Action<PosBusDisconnectError> OnPosBusDisconnected { get; set; }

        public void Init(string url);
        public void SetToken(string userToken, string userUUID, string sessionId);
        public void Connect();
        public void Disconnect();
        public bool IsConnected { get; }
        public bool ProcessMessageQueue { get; set; }
        public void ProcessReceivedMessagesFromMainThread();
        public unsafe void SendPosition(in UnityEngine.Vector3 pos);
        public void UnityReady();

        public void TriggerTeleport(in Guid target);
        public void TriggerInteractionMsg(uint kind, Guid targetID, int flag, string message);
        public void SendHandshake(string userToken, string userUUID, string sessionID, string URL = "");
    }

    public interface IPosBusMessage { }

    public class PosBusPosMsg : IPosBusMessage
    {
        public Guid userId;
        public Vector3 position;
        public PosBusPosMsg() { }
        public PosBusPosMsg(Guid id, Vector3 pos)
        {
            userId = id;
            position = pos;
        }
    }

    public class PosBusTransitionalEffectOnObjectMsg : IPosBusMessage
    {
        public Guid Emmiter;
        public Guid Object;
        public UInt32 Effect;
        public PosBusTransitionalEffectOnObjectMsg(Guid em, Guid obj, UInt32 eff)
        {
            Emmiter = em;
            Object = obj;
            Effect = eff;

        }
    }

    public class PosBusTransitionalBridgingEffectOnObjectMsg : IPosBusMessage
    {
        public Guid Emmiter;
        public Guid ObjectFrom;
        public Guid ObjectTo;
        public UInt32 Effect;
        public PosBusTransitionalBridgingEffectOnObjectMsg(Guid em, Guid objF, Guid objT, UInt32 eff)
        {
            Emmiter = em;
            ObjectFrom = objF;
            ObjectTo = objT;
            Effect = eff;

        }
    }

    public class PosBusTransitionalEffectOnPositionMsg : IPosBusMessage
    {
        public Guid Emmiter;
        public Vector3 Position;
        public UInt32 Effect;
        public PosBusTransitionalEffectOnPositionMsg(Guid em, Vector3 pos, UInt32 eff)
        {
            Emmiter = em;
            Position = pos;
            Effect = eff;

        }
    }

    public class PosBusTransitionalBridgingEffectOnPositionMsg : IPosBusMessage
    {
        public Guid Emmiter;
        public Vector3 PositionFrom;
        public Vector3 PositionTo;
        public UInt32 Effect;
        public PosBusTransitionalBridgingEffectOnPositionMsg(Guid em, Vector3 posFrom, Vector3 posTo, UInt32 eff)
        {
            Emmiter = em;
            PositionFrom = posFrom;
            PositionTo = posTo;
            Effect = eff;

        }
    }


    public class PosBusActiveObjectsPosMsg : IPosBusMessage
    {
        public Guid[] objectIds;
        public Vector3[] positions;
    }

    public class PosBusSelfPosMsg : IPosBusMessage
    {
        public Vector3 position;
    }

    public class PosBusRemoveUserMsg : IPosBusMessage
    {
        public Guid userId;
    }

    public class PosBusSignalMsg : IPosBusMessage
    {
        public PosBusSignalType signal;
    }

    public struct DecorationMetadata
    {
        public Guid assetID;
        public Vector3 position;
        public Vector3 rotation;
    }

    public class PosBusSetWorldMsg : IPosBusMessage
    {
        public Guid worldID;
        public Guid avatarControllerID;
        public Guid skyboxControllerID;
        public uint[] lodDistances;
        public DecorationMetadata[] decorations;
    }

    public class PosBusHighfiveMsg : IPosBusMessage
    {
        public Guid user1;
        public Guid user2;
    }

    public class PosBusFireworksMsg : IPosBusMessage
    {
        public Vector3 position;
    }

    public enum WowType
    {
        Positive, Negative
    }

    public class PosBusWowMsg : IPosBusMessage
    {
        public Guid structureId;
        public WowType type;
    }


    public struct TextureMetadata
    {
        public string label;
        public string data;
    }

    public struct AttributeMetadata
    {
        public string label;
        public Int32 attribute;
    }

    public struct StringMetadata
    {
        public string label;
        public string data;
    }

    public struct ObjectMetadata
    {
        public Guid objectId;
        public string name;
        public Vector3 position;
        public Guid parentId;
        public Guid assetType;
        public Guid infoUIType;
        public bool tetheredToParent;
        public bool isMinimap;
    }

    public class PosBusAddStaticObjectsMsg : IPosBusMessage
    {
        public ObjectMetadata[] objects;
    }
    public class PosBusSetStaticObjectPositionMsg : IPosBusMessage
    {
        public Guid objectId;
        public Vector3 position;
        public PosBusSetStaticObjectPositionMsg() { }
        public PosBusSetStaticObjectPositionMsg(Guid id, Vector3 pos)
        {
            objectId = id;
            position = pos;

        }
    }

    public enum PosBusSignalType
    {
        None = 0,
        DualConnection = 1, // Send from PosBus when we have another user connected with the same token
        Ready = 2, // Send from Unity when we spawned the world
        InvalidToken = 3, // Invald Token send on handshake
        Spawn = 4 // Send from PosBus, when the initial batch of World data is sent
    }

    public class PosBusRemoveStaticObjectsMsg : IPosBusMessage
    {
        public Guid[] objectIds;
    }

    public struct ActiveObjectMetadata
    {
        public Guid id;
        public Guid type;
        public string meta;
    }

    public class PosBusAddActiveObjectsMsg : IPosBusMessage
    {
        public ActiveObjectMetadata[] objects;
    }

    public class PosBusRemoveActiveObjectsMsg : IPosBusMessage
    {
        public Guid[] objectIds;
    }

    public class PosBusSetTexturesMsg : IPosBusMessage
    {
        public Guid objectID;
        public TextureMetadata[] textures;
    }

    public class PosBusSetAttributesMsg : IPosBusMessage
    {
        public Guid spaceID;
        public AttributeMetadata[] attributes;
    }

    public class PosBusSimpleNotificationMsg : IPosBusMessage
    {
        public PosBusAPI.Destination Destination;
        public UInt32 Kind;
        public Int32 Flag;
        public string Message;
    }

    public class PosBusRelayToReactMsg : IPosBusMessage
    {
        public string Target;
        public string Message;
    }



    public class PosBusSetStringsMsg : IPosBusMessage
    {
        public Guid spaceID;
        public StringMetadata[] strings;
    }

    public class PosBusObjectDefinition : IPosBusMessage
    {
        public ObjectMetadata metadata;
    }

    public enum PosBusDisconnectError
    {
        WRONG_TOKEN,
        NORMAL,
        UNKNOWN
    }


    public class PosBus : IPosBus
    {
        public bool HasReconnected { get; set; } = false;
        public bool TokenIsNotValid { get; set; } = false;

        public IWebsocketsHandler WebsocketHandler { get; set; }

        public bool IsConnected => _connected;
        private bool _connected = false;

        public bool ProcessMessageQueue { get; set; } = true;

        readonly private PosBusAPI.SendPositionMsg MyPosMsg;
        private ConcurrentQueue<IPosBusMessage> _receivedMessages;

        // this must be Invoked only from the Main Thread!
        public Action<IPosBusMessage> OnPosBusMessage { get; set; }
        public Action OnPosBusConnected { get; set; }
        public Action<PosBusDisconnectError> OnPosBusDisconnected { get; set; }

        private string _sessionId;
        private string _userUUID;
        private string _userToken;

        public PosBus(IWebsocketsHandler websocketHandler)
        {
            WebsocketHandler = websocketHandler;
            _receivedMessages = new ConcurrentQueue<IPosBusMessage>();
            MyPosMsg = new PosBusAPI.SendPositionMsg();
            _builder = new FlatBufferBuilder(1024);
        }

        public void Init(string url)
        {
            if (WebsocketHandler.IsInit && WebsocketHandler.GetState() == WebsocketHandlerState.Open)
            {
                Logging.Log("PosBus] We already have an open websocket, close first, before Init!", LogMsgType.NETWORKING);
                return;
            }

            WebsocketHandler.Init(url);

            SubscribeToWebSocketEvents();
        }

        public void SetToken(string userToken, string userUUID, string sessionId)
        {
            _sessionId = sessionId;
            _userToken = userToken;
            _userUUID = userUUID;

            TokenIsNotValid = false;
        }

        public void Connect()
        {
            if (WebsocketHandler.GetState() == WebsocketHandlerState.Open)
            {
                Logging.Log("[PosBus] Connection is already open..", LogMsgType.NETWORKING);
                return;
            }

            WebsocketHandler.Connect();
        }

        public void Disconnect()
        {
            if (WebsocketHandler == null || WebsocketHandler.GetState() == WebsocketHandlerState.Closed || WebsocketHandler.GetState() == WebsocketHandlerState.Closing) return;

            Logging.Log("[PosBus] Disconnecting...", LogMsgType.NETWORKING);

            WebsocketHandler.Close();

            _connected = false;
            ProcessMessageQueue = false;
            _receivedMessages = new ConcurrentQueue<IPosBusMessage>();

            OnPosBusDisconnected?.Invoke(PosBusDisconnectError.NORMAL);
        }

        public bool IsDisconnected()
        {
            return !(this.WebsocketHandler.GetState() == WebsocketHandlerState.Closing);
        }

        public void TriggerTeleport(in Guid target)
        {
            if (!_connected) return;
            WebsocketHandler.Send((new PosBusAPI.SwitchWorldMsg(target)).Buffer);
        }

        public void UnityReady()
        {
            if (!_connected) return;
            WebsocketHandler.Send((new PosBusAPI.SignalMsg(PosBusAPI.SignalType.SignalReady)).Buffer);
        }

        public void TriggerInteractionMsg(uint kind, Guid targetID, int flag, string message)
        {
            Debug.Log("Interaction Msg:" + " " + targetID.ToString() + " / " + kind + " / " + flag + " " + message);
            if (!_connected) return;
            WebsocketHandler.Send(new PosBusAPI.TriggerInteractionMsg(kind, targetID, flag, message).Buffer);
        }

        public void SendHandshake(string userToken, string userUUID, string sessionID, string URL = "")
        {
            if (!_connected) return;

            Logging.Log("[PosBus] Sending handshake: " + sessionID + "/" + userUUID + " / " + URL, LogMsgType.NETWORKING);

            var userTokeOffset = _builder.CreateString(userToken);
            var urlOffset = _builder.CreateString(URL);

            API.Handshake.StartHandshake(_builder);
            API.Handshake.AddHandshakeVersion(_builder, API.HandshakeVersion.v1);
            API.Handshake.AddProtocolVersion(_builder, API.ProtocolVersion.v1);
            API.Handshake.AddUserToken(_builder, userTokeOffset);
            var userIdOffset = SerializeID(userUUID);
            API.Handshake.AddUserId(_builder, userIdOffset);
            var sessionIdOffset = SerializeID(sessionID);
            API.Handshake.AddSessionId(_builder, sessionIdOffset);
            API.Handshake.AddUrl(_builder, urlOffset);
            var handshakeOffset = API.Handshake.EndHandshake(_builder);

            SendFlatBuffMsg(API.Msg.Handshake, handshakeOffset.Value);
        }

        // THIS ME BE CALLED ONLY FROM THE MAIN THREAD!
        public void ProcessReceivedMessagesFromMainThread()
        {
            if (!_connected || !ProcessMessageQueue) return;

            while (_receivedMessages.Count != 0)
            {
                IPosBusMessage msg;

                if (!ProcessMessageQueue)
                {
                    //Logging.Log("[PosBus] Processing of messages stopped while going through the queue..");
                    break;
                }

                if (_receivedMessages.TryDequeue(out msg))
                {
                    OnPosBusMessage?.Invoke(msg);
                }
            }
        }


        private void ProcessMessage(in byte[] msg)
        {
            var message = new PosBusAPI.PosBusMsg(msg);

            switch (message.MsgType)
            {
                case PosBusAPI.Msg.FlatBufferMessage:
                    {
                        var m = new PosBusAPI.FlatBufferMessage(message);
                        OnFlatBufferMessageReceived(m.Message());
                    }
                    break;

                case PosBusAPI.Msg.SendPosition:
                    {
                        var m = new PosBusAPI.SendPositionMsg(message);
                        EnqueueMessage(new PosBusSelfPosMsg()
                        {
                            position = m.Postion(),
                        });

                    }
                    break;
                case PosBusAPI.Msg.UsersPositions:
                    {
                        var m = new PosBusAPI.UsersPositionsMsg(message);
                        // .asUsersPositionsMsg();
                        var nUsers = m.NUsers();
                        for (int i = 0; i < nUsers; i++)
                        {
                            EnqueueMessage(m.GetElement(i));
                        }
                    }
                    break;
                case PosBusAPI.Msg.SimpleNotification:
                    {
                        var m = new PosBusAPI.SimpleNotificationMsg(message);
                        EnqueueMessage(m.Get());
                    }
                    break;
                case PosBusAPI.Msg.RelayToReact:
                    {
                        var m = new PosBusAPI.RelayToReactMsg(message);
                        EnqueueMessage(m.Get());
                    }
                    break;
                case PosBusAPI.Msg.RemoveStaticObjects:
                    {
                        var m = new PosBusAPI.RemoveStaticObjectsMsg(message);
                        var nObjects = m.NObjects();
                        var objectIds = new Guid[nObjects];

                        for (int i = 0; i < nObjects; i++)
                        {
                            objectIds[i] = m.ObjectId(i);
                        }
                        EnqueueMessage(new PosBusRemoveStaticObjectsMsg()
                        {
                            objectIds = objectIds
                        });
                        break;
                    }

                case PosBusAPI.Msg.SetStaticObjectPosition:
                    {
                        var m = new PosBusAPI.SetStaticObjectPositionMsg(message);
                        EnqueueMessage(m.Get());
                        break;
                    }
                case PosBusAPI.Msg.GoneUsers:
                    {
                        var m = new PosBusAPI.GoneUsersMsg(message);
                        var nUsers = m.NUsers();
                        for (int i = 0; i < nUsers; i++)
                        {
                            EnqueueMessage(new PosBusRemoveUserMsg()
                            {
                                userId = m.Id(i),
                            });
                        }
                        break;
                    }
                case PosBusAPI.Msg.Signal:
                    {
                        var m = new PosBusAPI.SignalMsg(message);
                        var signalType = (PosBusSignalType)m.Signal();
                        EnqueueMessage(new PosBusSignalMsg()
                        {
                            signal = signalType,
                        });
                        break;
                    }

                case PosBusAPI.Msg.TriggerTransitionalEffectsOnObject:
                    {
                        var m = new PosBusAPI.TriggerTransitionalEffectsOnObjectMsg(message);
                        var nEffects = m.NEffects();
                        for (int i = 0; i < nEffects; i++)
                        {
                            EnqueueMessage(m.GetElement(i));
                        };
                        // Example of using fields separately
                        // for (int i = 0; i < nEffects; i++)
                        // {
                        //     var Emmiter = m.Emmiter(i);
                        //     var Effect = m.Effect(i);
                        //     var Object = m.Object(i);

                        // }
                    }
                    break;

                case PosBusAPI.Msg.TriggerTransitionalBridgingEffectsOnObject:
                    {
                        var m = new PosBusAPI.TriggerTransitionalBridgingEffectsOnObjectMsg(message);
                        var nEffects = m.NEffects();
                        for (int i = 0; i < nEffects; i++)
                        {
                            EnqueueMessage(m.GetElement(i));
                        };
                    }
                    break;

                case PosBusAPI.Msg.TriggerTransitionalEffectsOnPosition:
                    {
                        var m = new PosBusAPI.TriggerTransitionalEffectsOnPositionMsg(message);
                        var nEffects = m.NEffects();
                        for (int i = 0; i < nEffects; i++)
                        {
                            EnqueueMessage(m.GetElement(i));
                        };
                    }
                    break;

                case PosBusAPI.Msg.TriggerTransitionalBridgingEffectsOnPosition:
                    {
                        var m = new PosBusAPI.TriggerTransitionalBridgingEffectsOnPositionMsg(message);
                        var nEffects = m.NEffects();
                        for (int i = 0; i < nEffects; i++)
                        {
                            EnqueueMessage(m.GetElement(i));
                        };
                        // Example of using fields separately
                        // for (int i = 0; i < nEffects; i++)
                        // {
                        //     var Emmiter = m.Emmiter(i);
                        //     var Effect = m.Effect(i);
                        //     var posFrom = m.PositionFrom(i);
                        //     var posTo = m.PositionTo(i);
                        // }
                    }
                    break;


                default:
                    Logging.Log("[PosBus] Unknown message kind", LogMsgType.NETWORKING);
                    break;
            }


        }

        public unsafe void SendPosition(in UnityEngine.Vector3 pos)
        {
            if (!_connected) return;

            MyPosMsg.SetPostion(pos);
            WebsocketHandler.Send(MyPosMsg.Buffer);
        }


        #region WebSocket Events

        private void SubscribeToWebSocketEvents()
        {
            WebsocketHandler.OnOpen += OnOpen;
            WebsocketHandler.OnClose += OnClose;
            WebsocketHandler.OnError += OnError;
            WebsocketHandler.OnMessage += OnMessage;
        }

        private void UnsubscribeWebSocketEvents()
        {
            WebsocketHandler.OnOpen -= OnOpen;
            WebsocketHandler.OnClose -= OnClose;
            WebsocketHandler.OnError -= OnError;
            WebsocketHandler.OnMessage -= OnMessage;
        }

        private void OnOpen()
        {
            _connected = true;

            Logging.Log("[PosBus] Connected", LogMsgType.NETWORKING);

            OnPosBusConnected?.Invoke();
        }

        // this will be call, if we have a non-voluntary disconnect(we did not call websocket.close() by ourself)
        private void OnClose(WebsocketHandlerCloseCode code)
        {
            Logging.Log("[PosBus] Websocket OnClose Event with code: " + code.ToString(), LogMsgType.NETWORKING);

            _connected = false;
            _receivedMessages = new ConcurrentQueue<IPosBusMessage>();

            if (code == WebsocketHandlerCloseCode.Normal)
            {
                OnPosBusDisconnected?.Invoke(PosBusDisconnectError.NORMAL);
            }
            else
            {
                OnPosBusDisconnected?.Invoke(PosBusDisconnectError.UNKNOWN);
            }
        }

        private void OnError(string err)
        {
            Logging.Log("[PosBus] Error: " + err, LogMsgType.NETWORKING);
        }

        private void OnMessage(byte[] msgInBytes)
        {
            ProcessMessage(msgInBytes);
        }

        #endregion

        void OnFlatBufferMessageReceived(FlatBuffMsg flatBuffMsg)
        {
            switch (flatBuffMsg.MsgType)
            {
                case API.Msg.SetWorld:
                    {
                        var setWorld = flatBuffMsg.Msg<API.SetWorld>().Value;
                        var decorations = new DecorationMetadata[setWorld.DecorationsLength];
                        for (int i = 0; i < setWorld.DecorationsLength; i++)
                        {
                            var decMeta = setWorld.Decorations(i).Value;
                            decorations[i].assetID = DeserializeID(decMeta.AssetId);
                            var posRot = decMeta.Pos.Value;
                            decorations[i].position.x = posRot.Pos.X;
                            decorations[i].position.y = posRot.Pos.Y;
                            decorations[i].position.z = posRot.Pos.Z;
                            decorations[i].rotation.x = posRot.Rot.X;
                            decorations[i].rotation.y = posRot.Rot.Y;
                            decorations[i].rotation.z = posRot.Rot.Z;
                        }
                        EnqueueMessage(new PosBusSetWorldMsg()
                        {
                            worldID = DeserializeID(setWorld.WorldId),
                            avatarControllerID = DeserializeID(setWorld.AvatarControllerId),
                            skyboxControllerID = DeserializeID(setWorld.SkyboxControllerId),
                            lodDistances = setWorld.GetLodDistancesArray(),
                            decorations = decorations
                        });
                        break;
                    }
                case API.Msg.On3DAction:
                    {
                        var action = flatBuffMsg.Msg<API.On3DAction>().Value;
                        switch (action.DataType)
                        {
                            case API.ActionMetadata.HighfiveMetadata:
                                var highfiveData = action.Data<API.HighfiveMetadata>().Value;
                                EnqueueMessage(new PosBusHighfiveMsg()
                                {
                                    user1 = DeserializeID(highfiveData.User1),
                                    user2 = DeserializeID(highfiveData.User2)
                                });
                                break;
                            case API.ActionMetadata.FireworksMetadata:
                                var fireworksData = action.Data<API.FireworksMetadata>().Value;
                                var pos = fireworksData.Position;
                                EnqueueMessage(new PosBusFireworksMsg()
                                {
                                    position = new Vector3(pos.Value.X, pos.Value.Y, pos.Value.Z),
                                });
                                break;
                            case API.ActionMetadata.WowMetadata:
                                var wowMetadata = action.Data<API.WowMetadata>().Value;
                                var structureId = DeserializeID(wowMetadata.StructureId);
                                var wowType = (WowType)wowMetadata.Type;
                                EnqueueMessage(new PosBusWowMsg()
                                {
                                    structureId = structureId,
                                    type = wowType
                                });
                                break;
                        }
                        break;
                    }
                case API.Msg.AddStaticObjects:
                    {

                        var addStaticObjects = flatBuffMsg.Msg<API.AddStaticObjects>().Value;
                        var objects = new ObjectMetadata[addStaticObjects.ObjectsLength];

                        for (int i = 0; i < objects.Length; i++)
                        {
                            var obj = addStaticObjects.Objects(i).Value;
                            objects[i].objectId = DeserializeID(obj.ObjectId);
                            objects[i].name = obj.Name;

                            var pos = obj.Position;
                            objects[i].position.x = pos.Value.X;
                            objects[i].position.y = pos.Value.Y;
                            objects[i].position.z = pos.Value.Z;

                            objects[i].parentId = DeserializeID(obj.ParentId);
                            objects[i].assetType = DeserializeID(obj.AssetType);
                            objects[i].tetheredToParent = obj.TetheredToParent;
                            objects[i].isMinimap = obj.Minimap;
                            objects[i].infoUIType = DeserializeID(obj.InfouiType);
                        }
                        EnqueueMessage(new PosBusAddStaticObjectsMsg()
                        {
                            objects = objects
                        });

                        break;
                    }

                case API.Msg.AddActiveObjects:
                    {
                        var addActiveObjects = flatBuffMsg.Msg<API.AddActiveObjects>().Value;
                        var objects = new ActiveObjectMetadata[addActiveObjects.ObjectsLength];
                        for (int i = 0; i < addActiveObjects.ObjectsLength; i++)
                        {
                            var obj = addActiveObjects.Objects(i).Value;
                            objects[i].id = DeserializeID(obj.Id);
                            objects[i].type = DeserializeID(obj.Type);
                            objects[i].meta = obj.Meta;
                        }
                        EnqueueMessage(new PosBusAddActiveObjectsMsg()
                        {
                            objects = objects
                        });
                        break;
                    }
                case API.Msg.SetObjectTextures:
                    {
                        var setTexture = flatBuffMsg.Msg<API.SetObjectTextures>().Value;

                        if (setTexture.ObjectsLength == 0) return;

                        var objectID = DeserializeID(setTexture.ObjectId);

                        TextureMetadata[] texturesMetaData = new TextureMetadata[setTexture.ObjectsLength];

                        for (int i = 0; i < setTexture.ObjectsLength; i++)
                        {
                            var textureDefinition = setTexture.Objects(i).Value;
                            texturesMetaData[i] = new TextureMetadata()
                            {
                                label = textureDefinition.Label,
                                data = textureDefinition.Data
                            };
                        }

                        EnqueueMessage(new PosBusSetTexturesMsg()
                        {
                            objectID = objectID,
                            textures = texturesMetaData
                        });
                        break;
                    }
                case API.Msg.SetObjectAttributes:
                    {
                        var setAttribute = flatBuffMsg.Msg<API.SetObjectAttributes>().Value;

                        if (setAttribute.ObjectsLength == 0) return;

                        var spaceID = DeserializeID(setAttribute.SpaceId);

                        AttributeMetadata[] attributesMetadata = new AttributeMetadata[setAttribute.ObjectsLength];

                        for (int i = 0; i < setAttribute.ObjectsLength; i++)
                        {
                            attributesMetadata[i] = new AttributeMetadata()
                            {
                                label = setAttribute.Objects(i).Value.Label,
                                attribute = setAttribute.Objects(i).Value.Attribute
                            };
                        }

                        EnqueueMessage(new PosBusSetAttributesMsg()
                        {
                            spaceID = spaceID,
                            attributes = attributesMetadata
                        });
                        break;
                    }
                case API.Msg.SetObjectStrings:
                    {
                        var setString = flatBuffMsg.Msg<API.SetObjectStrings>().Value;
                        if (setString.ObjectsLength == 0) return;
                        var spaceID = DeserializeID(setString.ObjectId);

                        StringMetadata[] stringMetadata = new StringMetadata[setString.ObjectsLength];

                        for (int i = 0; i < setString.ObjectsLength; i++)
                        {
                            stringMetadata[i] = new StringMetadata()
                            {
                                label = setString.Objects(i).Value.Label,
                                data = setString.Objects(i).Value.Data
                            };
                        }

                        EnqueueMessage(new PosBusSetStringsMsg()
                        {
                            spaceID = spaceID,
                            strings = stringMetadata
                        });
                        break;
                    }
                case API.Msg.ObjectDefinition:

                    var objectDefinition = flatBuffMsg.Msg<API.ObjectDefinition>().Value;

                    var objectMetadata = new ObjectMetadata();

                    objectMetadata.name = objectDefinition.Name;
                    objectMetadata.assetType = DeserializeID(objectDefinition.AssetType);
                    objectMetadata.objectId = DeserializeID(objectDefinition.ObjectId);
                    objectMetadata.parentId = DeserializeID(objectDefinition.ParentId);
                    objectMetadata.position = new Vector3(objectDefinition.Position.Value.X, objectDefinition.Position.Value.Y, objectDefinition.Position.Value.Z);
                    objectMetadata.isMinimap = objectDefinition.Minimap;
                    objectMetadata.infoUIType = DeserializeID(objectDefinition.InfouiType);
                    objectMetadata.tetheredToParent = objectDefinition.TetheredToParent;

                    EnqueueMessage(new PosBusObjectDefinition()
                    {
                        metadata = objectMetadata
                    });

                    break;
                default:
                    throw new UnityException("Received unknown flatbuffer message!");
            }
        }

        private void SendFlatBuffMsg(API.Msg msgType, int msgOffset)
        {
            FlatBuffMsg.StartFlatBuffMsg(_builder);
            FlatBuffMsg.AddMsgType(_builder, msgType);
            FlatBuffMsg.AddMsg(_builder, msgOffset);
            var flatBufMsgOffset = FlatBuffMsg.EndFlatBuffMsg(_builder);
            _builder.Finish(flatBufMsgOffset.Value);
            var m = new PosBusAPI.FlatBufferMessage(_builder.DataBuffer);
            _builder.Clear();

            WebsocketHandler.Send(m.Buffer);
        }

        private Offset<API.ID> SerializeID(string userUUID)
        {
            var userIdAsByteArray = Guid.Parse(userUUID).ToByteArray();
            var idOffset = API.ID.CreateID(_builder, BitConverter.ToUInt64(userIdAsByteArray, 0), BitConverter.ToUInt64(userIdAsByteArray, 8));
            return idOffset;
        }

        private static Guid DeserializeID(ID? userId)
        {
            var rawGuid = new byte[16];
            BitConverter.GetBytes(userId.Value.L).CopyTo(rawGuid, 0);
            BitConverter.GetBytes(userId.Value.M).CopyTo(rawGuid, 8);
            return new Guid(rawGuid);
        }

        void EnqueueMessage(IPosBusMessage message)
        {
            lock (_receivedMessages)
            {
                _receivedMessages.Enqueue(message);
            }
        }

        readonly FlatBufferBuilder _builder;
    }
}
