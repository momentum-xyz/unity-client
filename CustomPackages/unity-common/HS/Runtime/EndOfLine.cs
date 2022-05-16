using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLine : MonoBehaviour
{
    public bool DrawAtEnd;
    public bool processOnce = false;

    private Transform transformCache;

    LineRenderer _line;


    void Awake()
    {
        _line = GetComponentInParent<LineRenderer>();
    }


    void Update()
    {

        if (processOnce == false)
        {
            if (!DrawAtEnd)
                transform.position =
                    _line.useWorldSpace
                        ? _line.GetPosition(0)
                        : _line.transform.TransformPoint(_line.GetPosition(0));

            else
                transform.position =
                    _line.useWorldSpace
                        ? _line.GetPosition(_line.positionCount - 1)
                        : _line.transform.TransformPoint(_line.GetPosition(_line.positionCount - 1));

            processOnce = true;

        }
    }
}
