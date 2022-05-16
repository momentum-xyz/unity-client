using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    public class CameraDistance : MonoBehaviour
    {
        public float Distance = 5;
        public List<GameObject> DisableObjects;
        public List<Behaviour> DisableBehaviours;
        public float PollingDelay = 1;

        void OnEnable() => StartCoroutine( PollingLoop() );

        IEnumerator PollingLoop()
        {
            var pause = new WaitForSeconds( PollingDelay );
            while(true)
            {
                var cam = Camera.main?.transform;
                if( cam )
                {
                    var enable = (cam.position-transform.position).sqrMagnitude < Distance*Distance;
                    foreach( var op in DisableObjects ) op.SetActive( enable );
                    foreach( var beh in DisableBehaviours ) beh.enabled = enabled;
                }
                yield return pause;
            }
        }
    }
}