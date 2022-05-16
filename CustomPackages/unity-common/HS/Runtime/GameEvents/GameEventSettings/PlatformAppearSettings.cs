using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "Platform Appear Settings", menuName = "ScriptableObjects/Platform Appear Settings")]
    public class PlatformAppearSettings : ScriptableObject
    {
        public static PlatformAppearSettings Default;
        public bool IsDefault;

        public GameObject EffectsPrefab;

        public  void Awake()
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