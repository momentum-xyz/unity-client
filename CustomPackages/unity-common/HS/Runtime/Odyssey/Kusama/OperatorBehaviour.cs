using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;
using System;

public class OperatorBehaviour : MonoBehaviour, IEffectsTrigger, IWorldBehaviour, IStructureState
{
    public const int STAKE_REWARD_EFFECT_ID = 401;
    public GameObject stakeRewardFx;

    public List<LODSet> lodSets;

    private UserPlatformDriver userPlatformDriver;
    private AlphaStructureDriver driver;

    private Vector3 oldPosition;

    private bool lodSetsInitialized = false;
    private int stageMode = 0;

    void Awake()
    {
        driver = GetComponent<AlphaStructureDriver>();
    }

    public void InitBehaviour()
    {
        if (driver == null) return;

        userPlatformDriver = this.GetComponent<UserPlatformDriver>();

        Transform parentTransform = driver.parentTransform;

        if (parentTransform != null && userPlatformDriver != null)
        {
            // Look at the parent first, before setting up the tethers
            if (driver.LookAtParent)
                this.transform.LookAt(new Vector3(parentTransform.position.x, this.transform.position.y, parentTransform.transform.position.z));

            // Tethers

            HS.UserPlatformDriver parentPlatformDriver = parentTransform.GetComponent<UserPlatformDriver>();

            if (parentPlatformDriver != null)
            {
                userPlatformDriver.SetParent(parentPlatformDriver);
            }

            userPlatformDriver.UpdateTether();

            oldPosition = transform.position;
        }
    }

    public void UpdateBehaviour(float dt)
    {
        if (driver != null && userPlatformDriver != null && driver.parentTransform != null)
        {
            if (transform.position != oldPosition && userPlatformDriver != null)
            {
                userPlatformDriver.UpdateTether();
            }

            oldPosition = transform.position;
        }
    }

    public void FixedUpdateBehaviour(float dt)
    {

    }

    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {
        if (userPlatformDriver == null) return;

        userPlatformDriver.SetPrivacy(isPrivate, currentUserCanEnter);
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


    public T GetState<T>(string label)
    {
        if (typeof(T) == typeof(int))
        {
            if (label == "stagemode")
            {
                return (T)Convert.ChangeType(stageMode, typeof(T));
            }
        }

        return (T)Convert.ChangeType(-1, typeof(T));
    }

    public void SetState<T>(string label, T value)
    {
        if (label == "stagemode")
        {
            if (userPlatformDriver == null) return;
            stageMode = (int)Convert.ChangeType(value, typeof(int));
            userPlatformDriver.SetStageMode(stageMode > 0 ? true : false);
        }
    }

    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int type)
    {

    }

    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type)
    {

    }

    public void TriggerEffect(Vector3 source, int type)
    {

    }

    public void TriggerEffect(GameObject source, int type)
    {
        if (type == STAKE_REWARD_EFFECT_ID)
        {
            HS.Pool.Instance.GetSpawnFromPrefab(stakeRewardFx, source.transform);
        }
    }

}
