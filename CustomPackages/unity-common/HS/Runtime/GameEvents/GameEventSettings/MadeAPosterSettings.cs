using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    [CreateAssetMenu(fileName = "Made a Poster Settings", menuName = "ScriptableObjects/Made a Poster Settings")]
    public class MadeAPosterSettings : ScriptableObject
    {
        public static MadeAPosterSettings Default;
        public bool IsDefault;

        public GameObject EffectPrefab;
		

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