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
}

[CreateAssetMenu]
public class InjectAssetData : ScriptableObject
{
    public List<InjectedAsset> assetsToInject;
}
