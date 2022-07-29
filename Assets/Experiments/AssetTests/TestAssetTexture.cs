using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAssetTexture : MonoBehaviour, IScriptable
{
    public string textureLabel = "";

    public Guid Owner { get; set; }
    public IMomentumAPI API { get; set; }

    private int textureLOD = -1;

    public void Init()
    {

    }

    void Start()
    {
        API.RegisterForTextureLODUpdates(this, OnTextureLOD);
    }

    void OnDestroy()
    {
        API.UnregisterForTextureLODUpdates(this);
    }

    void OnTextureLOD(int objectLOD, int lod)
    {


    }

}
