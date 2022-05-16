using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IInfoUI_Texture
{
    public string GetLabel();
    public void SetTexture(Texture texture);
    public void ClearTexture();
}

public class InfoUI_Texture : MonoBehaviour, IInfoUI_Texture
{
    public string Label;
    public RawImage image;

    public string GetLabel()
    {
        return Label;
    }

    public void SetTexture(Texture texture)
    {
        if (image != null)
        {
            image.enabled = true;
            image.texture = texture;
        }
    }

    public void ClearTexture()
    {
        image.enabled = false;
        image.texture = null;
    }
}
