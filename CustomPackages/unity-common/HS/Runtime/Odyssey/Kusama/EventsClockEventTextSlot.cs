using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsClockEventTextSlot : MonoBehaviour, ITextSlot
{
    public string label = "time";

    HS.MultiSurfaceDriver _surfaceDriver;

    void Awake()
    {
        _surfaceDriver = GetComponent<HS.MultiSurfaceDriver>();
    }

    public string GetLabel()
    {
        return label;
    }

    public void SetText(string label, string text)
    {
        Debug.Log("Got event name: " + text);
        if (_surfaceDriver == null) return;

        _surfaceDriver.SetLabel(text);
    }

}
