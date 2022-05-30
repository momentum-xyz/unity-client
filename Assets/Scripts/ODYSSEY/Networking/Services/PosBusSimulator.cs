using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using System;

public class PosBusSimulator : IPosBus
{

    public IWebsocketsHandler WebsocketHandler { get; set; }
    public bool IsConnected { get; set; } = true;
    public bool ProcessMessageQueue { get; set; } = true;

    public Action<IPosBusMessage> OnPosBusMessage { get; set; }
    public Action OnPosBusConnected { get; set; }

#pragma warning disable CS0067
    public Action<PosBusDisconnectError> OnPosBusDisconnected { get; set; }
#pragma warning restore CS0067

    public void Connect()
    {
        OnPosBusConnected?.Invoke();
    }

    public void Disconnect()
    {
        OnPosBusDisconnected?.Invoke(PosBusDisconnectError.NORMAL);
    }

    public void Init(string url)
    {

    }

    public void ProcessReceivedMessagesFromMainThread()
    {

    }

    public void QueryPermission(in Guid space)
    {

    }

    public void SendHandshake(string userToken, string userUUID, string sessionID, string URL = "")
    {
        // OnPosBusMessage?.Invoke(new PosBusAssignWorldMsg()
        // {
        //     worldId = "d83670c7-a120-47a4-892d-f9ec75604f74",
        // });
    }

    public void SendPosition(in Vector3 pos)
    {

    }

    public void SetToken(string userToken, string userUUID, string sessionId)
    {

    }

    public void TriggerTeleport(in Guid target)
    {

    }
    public void SendInteractionTrigger(Guid space, string label, string metadata)
    {

    }

    public void TriggerInteractionMsg(uint kind, Guid targetID, int flag, string message)
    {

    }

    public void UnityReady()
    {

    }
}
