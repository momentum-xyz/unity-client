using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsClockTimeTextSlot : MonoBehaviour, ITextSlot
{
    public string label = "time";

    HS.MultiSurfaceDriver _surfaceDriver;

    TimeSpan _currentTime;
    bool _isCountingDown = false;
    float _deltaTimeSum = 0.0f;

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
        Debug.Log("Got Event Time: " + text);

        if (_surfaceDriver == null) return;

        _currentTime = TimeSpan.FromMilliseconds(int.Parse(text));

        _isCountingDown = true;

        _surfaceDriver.SetClock(_currentTime);
    }

    void Update()
    {
        if (!_isCountingDown) return;

        _deltaTimeSum += Time.deltaTime;

        if (_deltaTimeSum > 1.0f)
        {
            _currentTime = _currentTime.Subtract(TimeSpan.FromSeconds(_deltaTimeSum));
            _surfaceDriver.SetClock(_currentTime);
            _deltaTimeSum = 0.0f;
        }

    }

}
