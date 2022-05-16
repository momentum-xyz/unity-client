using HS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLODBehaviour : MonoBehaviour, IWorldBehaviour
{
    public List<LODSet> lodSets;
    private bool lodSetsInitialized = false;

    public void FixedUpdateBehaviour(float dt)
    {

    }

    public void InitBehaviour()
    {

    }

    public void UpdateBehaviour(float dt)
    {

    }

    public void UpdateLOD(int lodLevel)
    {
        if (lodSets.Count == 0 && !lodSetsInitialized)
        {
            FindLODSets();
            lodSetsInitialized = true;
        }

        foreach (HS.LODSet lodSet in lodSets)
        {
            lodSet.SetLOD(lodLevel);
        }

    }

    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {

    }

    void FindLODSets()
    {
        foreach (var l in GetComponentsInChildren<LODSet>(true))
        {
            lodSets.Add(l);
        }
    }
}
