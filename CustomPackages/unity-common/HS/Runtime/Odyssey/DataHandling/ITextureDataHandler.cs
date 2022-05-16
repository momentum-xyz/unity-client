using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public interface ITextureDataHandler
    {
        public void TextureUpdate(string label, Texture2D texture, float ratio = 1.0f);
    }
}