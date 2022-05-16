using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "GrabATable Settings", menuName = "ScriptableObjects/GrabATable Settings", order = 1)]
    public class GrabATableSettings : ScriptableObject
    {
        public static GrabATableSettings Default;

        public bool MoveAvatars = false;
        // public GameObject TablePrefab;
        public GameObject AppearEffectPrefab;
        public GameObject PlasmaballPrefab;
        public bool IsDefault;


        void Awake()
        {
            OnEnable();
        }
        
        void OnEnable()
        {
            // Debug.LogError( $"Starting {this} default: {IsDefault}" );
            if (IsDefault) Default = this;
        }
    }
}