using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITextureSlot 
{
    public abstract string GetLabel();
    public abstract void SetTexture(Texture2D texture);
    public abstract void SetTexture(Texture2D texture, float ratio);
}
