using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{

    public class SimpleOrbitByMouse : MonoBehaviour
    {
        public float ScrollSpeed = 60;
        public float MinDistance = 0.6f;
        Transform _cam;
        Vector3 _lMouse;

        void Update()
        {
            if (!_cam) _cam = Camera.main.transform;
            var nMouse = Input.mousePosition / Screen.width;

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.up, (nMouse.x - _lMouse.x) * 360, Space.World);
                transform.Rotate(Vector3.Cross(transform.position - _cam.position, _cam.up), (nMouse.y - _lMouse.y) * 360, Space.World);
            }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                var newDist =
                    Mathf.Max(
                        MinDistance,
                        _cam.localPosition.magnitude
                            * (-scroll * ScrollSpeed * Time.deltaTime + 1)
                    );
                _cam.localPosition = _cam.localPosition.normalized * newDist;
            }
            _lMouse = nMouse;
        }


    }
}
