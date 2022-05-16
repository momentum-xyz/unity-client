using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class DestroyOnParentDisable : MonoBehaviour
    {
        void OnDisable()
        {
            // Debug.Log( $"I got disabled so I'm destroying myself." );
            Destroy( gameObject );
        }
    }
}
