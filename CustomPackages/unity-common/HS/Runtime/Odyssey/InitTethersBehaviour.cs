using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;

/// <summary>
/// This Behaviour manages more than just the tethers, it also updates the platform colors and it also manages the visual updates of the privacy settings. 
/// This is so we save multiple GetComponent calls to find the UserPlatformDriver, if we separate all these into separate WorldBehaviours.
/// </summary>

public class InitTethersBehaviour : MonoBehaviour, IWorldBehaviour
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

        platformDriver = this.GetComponent<UserPlatformDriver>();

        Transform parentTransform = driver.parentTransform;

        if (parentTransform != null && platformDriver != null)
        {
            // Look at the parent first, before setting up the tethers
            if (driver.LookAtParent)
                this.transform.LookAt(new Vector3(parentTransform.position.x, this.transform.position.y, parentTransform.transform.position.z));

            // Tethers

            HS.UserPlatformDriver parentPlatformDriver = parentTransform.GetComponent<UserPlatformDriver>();

            if (platformDriver == null) return;

            if (parentPlatformDriver != null)
            {
                platformDriver.SetParent(parentPlatformDriver);
            }

            platformDriver.UpdateTether();
            oldPosition = transform.position;
        }


        // Platform Colors
        if (platformDriver != null)
        {
            string guidAsString = driver.guid.ToString();

            Color primaryColor = new Color(0, 0, 0);
            Color secondaryColor = new Color(0, 0, 0);

            ColorUtility.TryParseHtmlString("#" + guidAsString.Substring(1, 6), out primaryColor);
            ColorUtility.TryParseHtmlString("#" + guidAsString.Substring(24, 6), out secondaryColor);

            Color[] colors = { primaryColor, secondaryColor };

            platformDriver.SetColors(colors);
        }
    }

    public void UpdateBehaviour(float dt)
    {
        if (driver != null && platformDriver != null && driver.parentTransform != null)
        {
            if (transform.position != oldPosition && platformDriver != null)
            {
                platformDriver.UpdateTether();
            }

            oldPosition = transform.position;
        }
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
