using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IInfoUI_TextLabel
{
    public string Text { set; }
    public string Label { get; set; }
    public string DefaultValue { get; set; }
}

public class InfoUI_TextLabel : MonoBehaviour, IInfoUI_TextLabel
{
    public TextMeshProUGUI textMesh;

    [SerializeField]
    private string _defaultValue;

    [SerializeField]
    private string _label;

    public string Text
    {
        set
        {
            textMesh.text = value;
        }
    }

    public string DefaultValue
    {
        get
        {
            return _defaultValue;
        }

        set { }
    }

    public string Label { get { return _label; } set { _label = value; } }

    void Awake()
    {
        if (textMesh != null) textMesh = GetComponent<TextMeshProUGUI>();
    }


}
