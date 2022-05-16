using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "High Five Settings", menuName = "ScriptableObjects/High Five Settings", order = 2)]
    public class HighFiveSettings :ScriptableObject
    {
        public static HighFiveSettings Default;
 
 
        public GameObject HighFiveEffectPrefab;
		public GameObject ArcPrefab;

        public bool IsDefault = false;

        public void Awake()
        {
            OnEnable();
        }

         public void OnEnable()
         {
             if (IsDefault)
                 Default = this;
         }
    }
}