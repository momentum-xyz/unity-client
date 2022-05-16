using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextureSlot : MonoBehaviour, ITextureSlot
{
    public string Label;
    public string textureUniformName = "_BaseMap";
    public MeshRenderer renderer;

    void Awake()
    {
        if(renderer == null)
        {
            renderer = GetComponent<MeshRenderer>();
        }
    }

    public void SetTexture(Texture2D texture)
    {
        renderer.material.SetTexture(textureUniformName, texture);
    }

    public void SetTexture(Texture2D texture, float ratio)
    {
        SetTexture(texture);
    }

    public string GetLabel()
    {
        return Label;
    }
}
