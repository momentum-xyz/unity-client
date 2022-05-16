using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    /// <summary>
    /// This class is used in prefabs to set the maximum of a prefab in the scene
    /// </summary>
    public class Poolable : MonoBehaviour
    {
        public int MaxAmount = 10;
        public GameObject Origin{get;set;} // the prefab that spawned this object (once instatiated)



        // Since we could be accidentally destroyed by some other method, (fi because a scne gets unloaded)
        // we should make sure the Pool stops tracking us.
        void OnDestroy()
        {
            HS.Pool.Instance?.Untrack( this );
        }
    }
}