using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "Made a Meme Settings", menuName = "ScriptableObjects/Made a Meme Settings")]
    public class MadeAMemeSettings : ScriptableObject
    {
        public static MadeAMemeSettings Default;
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