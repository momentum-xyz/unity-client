using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class MorphElement : MonoBehaviour
    {
        Animator _anm;

        void OnEnable()
        {
            MorphManager.Register(this);
        }

        void OnDisable()
        {
            MorphManager.Deregister(this);
        }

        public void UpdatePhase(float newPhase)
        {
            if (!_anm) _anm = GetComponent<Animator>();
            if (!_anm)
            {
                Debug.LogError("MorphElement [{gameObject.name}] has no animator. Disabling.");
                enabled = false;
                return;
            }
            _anm.SetFloat("morph", newPhase);
        }
    }
}