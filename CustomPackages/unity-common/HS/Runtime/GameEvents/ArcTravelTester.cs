using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    public class ArcTravelTester : MonoBehaviour
    {
        public GameObject Avatar;
        public Collider TestingSurface;
        public ArcTravelSettings Settings;


        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;
                if (TestingSurface.Raycast(ray, out info, 200))
                {
                    ArcTravel.Create(Avatar, info.point + Vector3.up * 0.7f).Settings = Settings;
                    // Avatar.transform.position = info.point + Vector3.up * 0.7f;
                }
            }
        }
    }
}