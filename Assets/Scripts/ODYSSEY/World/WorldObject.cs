using System;
using System.Collections.Generic;

using HS;
using Odyssey;
using UnityEngine;

public enum TextureDataState
{
    NOTLOADED,
    DOWNLOADING,
    DOWNLOADED,
    ERROR
}
public class TextureData
{
    public string label;
    public string originalHash;
    public string lodHash;
    public TextureDataState state = TextureDataState.NOTLOADED;
}

public class TextLabelData
{
    public string label;
    public string text;
}

public enum WorldObjectState
{
    ADDED,
    DOWNLOADING_ASSET,
    SPAWNED,
    NO_ASSET_FOUND
}

public class WorldObject : IInfoUIHovarable
{
    public WorldObjectState state = WorldObjectState.ADDED;
    public string name;                         // structure name
    public Guid guid { get; set; }                          // object's own guid
    public Guid parentGuid;                     // parent uid
    public Guid spaceTypeId;                    // type of structure - Program, Challenge, etc
    public Guid uiAssetGuid { get; set; } = Guid.Empty;        // type of asset to use for the 3D UI (Info UI)
    public Vector3 position;                    // position of this structures
    public Dictionary<string, TextureData> textures = new Dictionary<string, TextureData>();
    public Dictionary<string, TextLabelData> textlabels = new Dictionary<string, TextLabelData>();
    public string assetSubtype;                 // used to determine model of structure
    public int privateMode;                    // is this platform shielded? 
    public Guid assetGuid;                      // the GUID of the asset that should be instantiated!
    public GameObject GO;
    private AlphaStructureDriver _structureDriver = null;
    public bool showOnMiniMap = true;

    public int texturesLOD = 1; // the last level by default
    public int LOD = 3; // the last level by default
    public bool texturesLoaded = false;
    public bool texturesDirty = false;
    public bool hasFullMetadata = false;    // a flag that shows if the object has received it's full metadata information
    public bool onlyHighQualityTextures = false;
    public bool alwaysUpdateTextures = false;
    public Vector3 PositionInWorld { get { return WorldPosition(); } set { } }

    public AlphaStructureDriver GetStructureDriver()
    {
        if (GO == null) return null;

        if (_structureDriver == null)
        {
            _structureDriver = GO.GetComponent<AlphaStructureDriver>();
        }

        return _structureDriver;
    }

    public TextureData GetTextureDataForLabel(string label)
    {
        TextureData td = null;
        textures.TryGetValue(label, out td);
        return td;
    }

    public override string ToString()
    {

        string s = name + " => " + guid.ToString() + "\n";

        return s;

    }

    public Vector3 WorldPosition()
    {
        AlphaStructureDriver driver = GetStructureDriver();

        if (driver != null && driver.customCenter != null)
        {
            return driver.customCenter.position;
        }

        return position;

    }
}
