using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "Join The Plan Settings", menuName = "ScriptableObjects/Join The Plan Settings")]
    public class JoinThePlanSettings : ScriptableObject
    {
        public static JoinThePlanSettings Default;
        
		
		public GameObject AvatarEffect;
		public GameObject ConnectionEffect;
		public GameObject TeamSpaceEffect;
        
		
		
		public bool IsDefault;

        public  void Awake()
        {
            OnEnable();
        }
        
        private void OnEnable()
        {
            if (IsDefault)
                Default = this;
        }
    }
}