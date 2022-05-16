using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS
{
    [CreateAssetMenu(fileName = "Plasma Tether Settings", menuName = "ScriptableObjects/Plasma Tether Settings", order = 1)]
    public class PlasmaTetherSettings : ScriptableObject
    {
        public static PlasmaTetherSettings Default;
        public bool IsDefault;
		public GameObject Prefab;

        void Awake()        { if( IsDefault ) Default = this;}
        void OnEnable()     { if( IsDefault ) Default = this;}
    }
}