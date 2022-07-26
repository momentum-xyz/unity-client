using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using System;

public class MockPosBus : IPosBus
{
    public bool HasReconnected { get; set; } = false;
    public bool TokenIsNotValid { get; set; } = false;
    public IWebsocketsHandler WebsocketHandler { get; set; }
    public MockData Data { get; set; }
    public Action<IPosBusMessage> OnPosBusMessage { get; set; }
    public Action OnPosBusConnected { get; set; }
    public Action<PosBusDisconnectError> OnPosBusDisconnected { get; set; }

    public bool IsConnected => true;

    public bool ProcessMessageQueue { get; set; } = true;

    public void Connect()
    {
        OnPosBusConnected?.Invoke();
    }

    public void Disconnect()
    {

    }

    public void Init(string url)
    {

    }

    public void ProcessReceivedMessagesFromMainThread()
    {

    }

    public void SendHandshake(string userToken, string userUUID, string sessionID, string URL = "")
    {
        Debug.Log("Got handshake!");

        PosBusSetWorldMsg posBusSetWorldMsg = new PosBusSetWorldMsg();
        posBusSetWorldMsg.avatarControllerID = Guid.Parse(Data.AvatarControllerID);
        posBusSetWorldMsg.lodDistances = new uint[] { (uint)Data.LOD1, (uint)Data.LOD2, (uint)Data.LOD3 };
        posBusSetWorldMsg.worldID = Guid.Parse(Data.WorldID);
        posBusSetWorldMsg.skyboxControllerID = Guid.Parse(Data.SkyboxID);

        if (Data.worldDecorations.Count > 0)
        {
            DecorationMetadata[] decorations = new DecorationMetadata[Data.worldDecorations.Count];
            for (var i = 0; i < Data.worldDecorations.Count; ++i)
            {
                decorations[i] = new DecorationMetadata()
                {
                    assetID = Guid.Parse(Data.worldDecorations[i].assetID),
                    position = Data.worldDecorations[i].position,
                    rotation = Vector3.zero
                };
            }
            posBusSetWorldMsg.decorations = decorations;
        }



        OnPosBusMessage?.Invoke(posBusSetWorldMsg);

        // Send user spawn position
        OnPosBusMessage?.Invoke(new PosBusSelfPosMsg()
        {
            position = Data.userSpawnPosition
        });

        if (Data.spaces.Count > 0)
        {
            ObjectMetadata[] objs = new ObjectMetadata[Data.spaces.Count];
            // Start sending AddStaticObjectMessage
            for (var i = 0; i < Data.spaces.Count; ++i)
            {
                MockSpaceData sd = Data.spaces[i];

                ObjectMetadata objectMetadata = new ObjectMetadata();
                objectMetadata.objectId = Guid.Parse(sd.ID);
                objectMetadata.assetType = Guid.Parse(sd.assetTypeID);
                objectMetadata.name = sd.name;
                objectMetadata.infoUIType = sd.uiAssetID.Length > 0 ? Guid.Parse(sd.uiAssetID) : Guid.Empty;
                objectMetadata.position = sd.position;
                objectMetadata.parentId = sd.parentID.Length > 0 ? Guid.Parse(sd.parentID) : Guid.Empty;
                objectMetadata.isMinimap = sd.showMinimap;

                objs[i] = objectMetadata;
            }

            OnPosBusMessage?.Invoke(new PosBusAddStaticObjectsMsg()
            {
                objects = objs
            });
        }

    }

    public void SendInteractionTrigger(Guid space, string label, string metadata)
    {

    }

    public void SendPosition(in Vector3 pos)
    {

    }

    public void SetToken(string userToken, string userUUID, string sessionId)
    {

    }

    public void TriggerInteractionMsg(uint kind, Guid targetID, int flag, string message)
    {

    }

    public void TriggerTeleport(in Guid target)
    {

    }

    public void UnityReady()
    {

    }
}
