using System;
using Odyssey;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapWisp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField]
    GameObject HoverEffect;

    [SerializeField]
    GameObject SelectedPanel;

    [SerializeField]
    TextMeshProUGUI PlayerNameLabel;

    [Header("Settings")]
    [SerializeField]
    float DoubleClickTime = .4f;

    public Action<MinimapWisp> OnDoubleClicked;
    public Action<MinimapWisp> OnSelected;

    public WispData Data { get; set; }

    public void SetPlayerName(string name)
    {
        PlayerNameLabel.text = name;
    }

    #region Event Handlers

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverEffect.SetActive(true);
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.fullyExited) return;
        HoverEffect.SetActive(false);
    }

    public void OnClick(BaseEventData e)
    {
        var pointerEventData = (PointerEventData)e;

        var currentTime = Time.unscaledTime;
        if (currentTime - _lastClickTime < DoubleClickTime)
        {
            OnDoubleClicked?.Invoke(this);
            SelectedPanel.SetActive(true);
        }
        else if (pointerEventData.pointerClick.name != "PlayerNameplate")
        {
            SelectedPanel.SetActive(!SelectedPanel.activeSelf);
            RaiseOnSelected();
        }
        _lastClickTime = currentTime;
    }

    void RaiseOnSelected()
    {
        if (SelectedPanel.activeSelf)
        {
            OnSelected?.Invoke(this);
        }
    }


    #endregion

    float _lastClickTime;
}
