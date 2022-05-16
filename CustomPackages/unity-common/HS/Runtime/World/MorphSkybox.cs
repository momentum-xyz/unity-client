using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    [ExecuteInEditMode]
    public class MorphSkybox : MonoBehaviour
    {
        public Color Color;
        public Color FogColor;



        void Update()
        {
            RenderSettings.skybox.SetColor("_Tint", Color);
            RenderSettings.fogColor = FogColor;
        }
    }
}