using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InjectedAsset
{
    public Vector3 position;
    public string name;
    public string GUID;
    public GameObject prefab;
    public List<InjectedTextureData> textureData;
}

[System.Serializable]
public class InjectedTextureData
{
    public string label;
    public string hash;
}

[CreateAssetMenu]
public class InjectAssetData : ScriptableObject
{
    public List<InjectedAsset> assetsToInject;
}
