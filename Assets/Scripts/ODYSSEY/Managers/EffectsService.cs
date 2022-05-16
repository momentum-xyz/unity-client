using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Odyssey
{
    public interface IEffectsService
    {
        public void MemeUploadedFX(Vector3 position, Texture2D image);
        public void PosterUploadedFX(Vector3 position, Texture2D image);
    }

    public class EffectsService : IEffectsService
    {

        public void MemeUploadedFX(Vector3 position, Texture2D image)
        {
            HS.MadeAMemeEvent.Create(position, image);
        }

        public void PosterUploadedFX(Vector3 position, Texture2D image)
        {
            HS.MadeAPosterEvent.Create(position, image);
        }

    }
}

