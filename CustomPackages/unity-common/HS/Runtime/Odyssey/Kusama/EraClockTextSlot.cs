using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraClockTextSlot : MonoBehaviour, ITextSlot
{
    public string label = "kusama_clock_era_time";

    HS.MultiSurfaceDriver _surfaceDriver;

    TimeSpan _currentTime;
    bool _isCountingDown = false;

    float deltaTimeSum = 0.0f;

    void Awake()
    {
        _surfaceDriver = GetComponent<HS.MultiSurfaceDriver>();
    }

    public string GetLabel()
    {
        return label;
    }

    public void SetText(string label, string text)
    {
        Debug.Log("Got Era Time: " + text);
        if (_surfaceDriver == null) return;

        _currentTime = TimeSpan.FromMilliseconds(int.Parse(text));

        _isCountingDown = true;

        _surfaceDriver.SetClock(_currentTime);

    }

    void Update()
    {
        if (!_isCountingDown) return;

        deltaTimeSum += Time.deltaTime;

        if(deltaTimeSum > 1.0f)
        {
            _currentTime = _currentTime.Subtract(TimeSpan.FromSeconds(deltaTimeSum));
            _surfaceDriver.SetClock(_currentTime);
            deltaTimeSum = 0.0f;
        }
        
    }



}
