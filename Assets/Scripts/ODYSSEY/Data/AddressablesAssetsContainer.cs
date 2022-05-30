using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AddressablesAsset
{
    public string address;
    public string guid;
    public string prefabPath;
}


[CreateAssetMenu]
public class AddressablesAssetsContainer : ScriptableObject
{
    public GenericDictionary<string, List<AddressablesAsset>> assets;
    public List<AddressablesAsset> sharedAssets;
}
