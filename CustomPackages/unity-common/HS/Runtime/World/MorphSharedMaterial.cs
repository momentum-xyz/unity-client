using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    [ExecuteInEditMode]
    public class MorphSharedMaterial : MonoBehaviour
    {
        public Material SharedMaterial;
        public Color Color;
        public bool AlsoShiftEmission;
        public Color EmissionColor;



        void Update()
        {
            SharedMaterial.color = Color;
            if (AlsoShiftEmission)
                SharedMaterial.SetColor("_EmissionColor", EmissionColor);
        }

    }
}

