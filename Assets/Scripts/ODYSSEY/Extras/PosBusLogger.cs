using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using Odyssey.Networking;
using System;

public class PosBusLogger
{
    public static Action<IPosBusMessage> OnPosBusMessageBridge;
    public static IPosBus PosBus = null;

    public static void Init(IPosBus posbus)
    {
        PosBus = posbus;
        PosBus.OnPosBusMessage += OnPosBusMsg;
    }

    public static void Destroy()
    {
        PosBus.OnPosBusMessage -= OnPosBusMsg;
        PosBus = null;
    }

    public static void OnPosBusMsg(IPosBusMessage msg)
    {
        OnPosBusMessageBridge?.Invoke(msg);
    }
}
