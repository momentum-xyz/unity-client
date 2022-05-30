using System.Collections.Generic;
using UnityEngine;
using System;
using Odyssey;

public class WorldDefinition
{

    public const float Default_LOD1Distance = 6400.0f;
    public const float Default_LOD2Distance = 14400.0f;
    public const float Default_LOD3Distance = 16900.0f;

    public const string Default_AvatarController = "00000000-0000-0000-0001-000000000010";


    public string worldName;                            // for humans only
    public Guid worldGuid;                              // Guid for the world
    public List<WorldDecoration> worldDecorations;          // decorations
    public Guid worldAvatarController;                        // contains the controller for the world
    public Guid worldSkyboxController;                  // contains the skybox manager for the world
    public float LOD1Distance;                          // distance when LODs are used
    public float LOD2Distance;                          // distance when LODs are used
    public float LOD3Distance;                          // distance when LODs are used
    public string spawnSpaceId;
    public Guid assetGuid;

}

public class WorldDecoration
{
    public Guid guid;
    public Vector3 position;
    public Vector3 rotation;

    public WorldDecoration(Guid guid, Vector3 position, Vector3 eulerRotation)
    {
        this.guid = guid;
        this.position = position;
        this.rotation = eulerRotation;
    }
}
