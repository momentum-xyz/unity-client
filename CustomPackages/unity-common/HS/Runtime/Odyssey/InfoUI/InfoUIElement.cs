using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UILOD
{
    public GameObject uiContainer;
}

public class InfoUIElement : MonoBehaviour
{
    public int[] lodDistances;
    public List<UILOD> lods;

    public Action<Guid, string> OnLabelClicked;

    [System.NonSerialized]
    public bool InUse = false;

    [System.NonSerialized]
    public Guid guid;

    [System.NonSerialized]
    public Guid assetGuid;

    public Dictionary<string, List<IInfoUI_Texture>> Textures => _textures;
    public Dictionary<string, List<IInfoUI_TextLabel>> TextLabels => _textLabels;

    private Dictionary<string, List<IInfoUI_TextLabel>> _textLabels = new Dictionary<string, List<IInfoUI_TextLabel>>();

    private IInfoUI_Clickable[] clickables = null;

    private Dictionary<string, List<IInfoUI_Texture>> _textures = new Dictionary<string, List<IInfoUI_Texture>>();

    private int lastLOD = -1;

    void Awake()
    {
        IInfoUI_TextLabel[] textLabelsComp = GetComponentsInChildren<IInfoUI_TextLabel>(true);

        for (var i = 0; i < textLabelsComp.Length; ++i)
        {
            if (!_textLabels.ContainsKey(textLabelsComp[i].Label))
            {
                _textLabels[textLabelsComp[i].Label] = new List<IInfoUI_TextLabel>();
            }

            _textLabels[textLabelsComp[i].Label].Add(textLabelsComp[i]);
        }

        clickables = GetComponentsInChildren<IInfoUI_Clickable>(true);

        if (clickables != null)
        {
            for (var i = 0; i < clickables.Length; ++i)
            {
                clickables[i].OnClicked += OnClicked;
            }
        }

        var texturesComp = GetComponentsInChildren<IInfoUI_Texture>(true);

        for (var i = 0; i < texturesComp.Length; ++i)
        {
            if (!_textures.ContainsKey(texturesComp[i].GetLabel()))
            {
                _textures[texturesComp[i].GetLabel()] = new List<IInfoUI_Texture>();
            }

            _textures[texturesComp[i].GetLabel()].Add(texturesComp[i]);
        }
    }

    void OnClicked(string label)
    {
        OnLabelClicked?.Invoke(guid, label);
    }

    private void OnDestroy()
    {
        if (clickables != null)
        {
            for (var i = 0; i < clickables.Length; ++i)
            {
                clickables[i].OnClicked -= OnClicked;
            }
        }
    }

    public int CalcLOD(float distanceSq)
    {
        if (lodDistances.Length == 0) return 0;

        int lod = lodDistances.Length - 1; // by default we are at largest available LOD;

        for (var i = lodDistances.Length - 1; i >= 0; i--)
        {
            if (distanceSq < lodDistances[i])
            {
                lod = i;
            }
        }

        return lod;
    }

    public void SetLOD(float distanceSq)
    {
        if (lods == null || lods.Count == 0) return;

        int lod = CalcLOD(distanceSq);

        if (lod == lastLOD) return;

        if (lod > lods.Count - 1) lod = lods.Count - 1;

        for (var i = 0; i < lods.Count; ++i)
        {
            lods[i].uiContainer.SetActive(lod == i);
        }

        lastLOD = lod;
    }


    public void UpdateTextLabels(string label, string value)
    {
        if (_textLabels.ContainsKey(label))
        {
            for (var i = 0; i < _textLabels[label].Count; ++i)
            {
                _textLabels[label][i].Text = value;
            }

        }
    }

    public void UpdateTextures(string label, Texture texture)
    {
        if (!_textures.ContainsKey(label)) return;

        for (var i = 0; i < _textures[label].Count; ++i)
        {
            _textures[label][i].SetTexture(texture);
        }
    }

}
