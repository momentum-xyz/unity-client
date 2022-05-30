using Odyssey;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MockSpaceData
{
    public string name = "";
    public string ID = "";
    public string parentID = "";
    public string assetTypeID = "";
    public string uiAssetID = "";
    public Vector3 position;
    public bool showMinimap = false;

}

[System.Serializable]
public class MockWorldDecorationData
{
    public string name;
    public string assetID;
    public Vector3 position;
}

[CreateAssetMenu]
public class MockData : ScriptableObject
{
    [Header("World Definition")]
    public string WorldID;
    public int LOD1 = 0;
    public int LOD2 = 0;
    public int LOD3 = 0;
    public string AvatarControllerID;
    public string SkyboxID;
    public List<MockWorldDecorationData> worldDecorations;


    [Header("User")]
    public Vector3 userSpawnPosition = Vector3.zero;

    [Header("World Structure")]
    public List<MockSpaceData> spaces;

    public virtual void Init(IMomentumContext context)
    {

    }
}
