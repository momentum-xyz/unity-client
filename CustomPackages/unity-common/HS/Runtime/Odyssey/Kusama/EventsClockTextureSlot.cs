using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsClockTextureSlot : MonoBehaviour, ITextureSlot
{
    public string label = "texture";

    HS.MultiSurfaceDriver _surfaceDriver;

    void Awake()
    {
        _surfaceDriver = GetComponent<HS.MultiSurfaceDriver>();
    }

    public string GetLabel()
    {
        return label;
    }

    public void SetTexture(Texture2D texture)
    {
        _surfaceDriver.SetScreen(texture);
    }

    public void SetTexture(Texture2D texture, float ratio)
    {
        _surfaceDriver.SetScreen(texture);
    }


}
