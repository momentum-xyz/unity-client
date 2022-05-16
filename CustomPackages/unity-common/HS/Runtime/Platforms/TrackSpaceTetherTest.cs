using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class TrackSpaceTetherTest : MonoBehaviour
    {
        public Transform Target;
        public float Radius = 20;


        void Start()
        {
            GetComponent<TrackSpaceDriver>().Setup( Target, Radius );
        }
    }
}