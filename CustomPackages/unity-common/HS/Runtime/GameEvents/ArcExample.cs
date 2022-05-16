using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    public class ArcExample : MonoBehaviour
    {
        [SerializeField] private GameObject endObject;

        // Start is called before the first frame update
        void Start()
        {
            ArcTravel arcTravel = ArcTravel.Create(this.gameObject, endObject.transform.position);
        }
    }
}