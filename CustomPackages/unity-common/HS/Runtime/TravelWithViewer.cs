using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelWithViewer : MonoBehaviour
{
    public bool KeepInitialHeight = true;
    public Transform Height;
    Transform _cam;
    float _height;

    void OnEnable()
    {
        if (!Height) Height = transform;
        _height = Height.position.y;
        _cam = Camera.main?.transform;
    }


    void Update()
    {
        if (!_cam)
        {
            if (Camera.main != null)
            {
                _cam = Camera.main.transform;
            }
            else
            {
                return;
            }
        }

        transform.position =
            KeepInitialHeight
            ? new Vector3(
                    _cam.position.x,
                    _height,
                    _cam.position.z
                )
            : _cam.position;
    }
}
