using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;


public class VatPlatformBehaviour : MonoBehaviour, IWorldBehaviour
{
    public List<LODSet> lodSets;
    private AlphaStructureDriver driver;

    private Vector3 oldPosition;
    private UserPlatformDriver platformDriver;

    private bool lodSetsInitialized = false;

    void Awake()
    {
        driver = GetComponent<AlphaStructureDriver>();
    }

    public void InitBehaviour()
    {
        if (driver == null) return;

        Transform parentTransform = driver.parentTransform;

        platformDriver = this.GetComponent<UserPlatformDriver>();

        // Look at the parent first, before setting up the tethers
        if (driver.LookAtParent) this.transform.LookAt(new Vector3(parentTransform.position.x, this.transform.position.y, parentTransform.transform.position.z));

    }


    public void UpdateBehaviour(float dt)
    {

    }

    public void FixedUpdateBehaviour(float dt)
    {

    }

    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {
        if (platformDriver == null) return;

        platformDriver.SetPrivacy(isPrivate, currentUserCanEnter);
    }

    public void UpdateLOD(int lodLevel)
    {
        if (platformDriver == null) return;

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

    void FindLODSets()
    {
        foreach (var l in GetComponentsInChildren<LODSet>(true))
        {
            lodSets.Add(l);
        }
    }
}
