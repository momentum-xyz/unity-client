using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IInfoUI_Clickable
{
    public Action<string> OnClicked { get; set; }
}

[RequireComponent(typeof(EventTrigger))]
public class InfoUI_Clickable : MonoBehaviour, IInfoUI_Clickable
{
    public string Label = "";

    public Action<string> OnClicked { get; set; }

    public void Clicked()
    {
        OnClicked?.Invoke(Label);
    }
}
