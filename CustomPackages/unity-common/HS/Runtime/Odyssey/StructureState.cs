using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;
using System;

public class StructureState : MonoBehaviour, IStructureState
{
    private int stageMode = 0;
    private UserPlatformDriver userPlatformDriver;

    void Awake()
    {
        userPlatformDriver = GetComponent<UserPlatformDriver>();
    }


    public T GetState<T>(string label)
    {
        if (typeof(T) == typeof(int))
        {
            if (label == "stagemode")
            {
                return (T)Convert.ChangeType(stageMode, typeof(T));
            }
        }

        return (T)Convert.ChangeType(-1, typeof(T));
    }

    public void SetState<T>(string label, T value)
    {
        if (label == "stagemode")
        {
            stageMode = (int)Convert.ChangeType(value, typeof(int));
            userPlatformDriver.SetStageMode(stageMode > 0 ? true : false);
        }
    }
}
