using HS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ValidatorNodeBehaviour : MonoBehaviour, IWorldBehaviour, IStructureState, IEffectsTrigger
{
    public const int STAKE_REWARD_EFFECT_ID = 401;
    public GameObject stakeEffectFx;

    public List<LODSet> lodSets;

    private AlphaStructureDriver driver;
    private UserPlatformDriver userPlatformDriver;
    private NodeStateVisualsDriver nodeVisualDriver;

    private Vector3 oldPosition;
    private bool lodSetsInitialized = false;


    void Awake()
    {
        driver = GetComponent<AlphaStructureDriver>();
    }

    public void InitBehaviour()
    {
        if (driver == null) return;

        userPlatformDriver = GetComponent<UserPlatformDriver>();
        nodeVisualDriver = GetComponent<NodeStateVisualsDriver>();

        Transform parentTransform = driver.parentTransform;

        if (parentTransform != null && userPlatformDriver != null)
        {
            // Look at the parent first, before setting up the tethers
            if (driver.LookAtParent)
                this.transform.LookAt(new Vector3(parentTransform.position.x, this.transform.position.y, parentTransform.transform.position.z));

            // Tethers

            HS.UserPlatformDriver parentPlatformDriver = parentTransform.GetComponent<UserPlatformDriver>();

            if (userPlatformDriver == null) return;

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


    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {

    }
    public T GetState<T>(string label)
    {
        return (T)Convert.ChangeType(-1, typeof(T));
    }

    public void SetState<T>(string label, T value)
    {
        if (typeof(T) == typeof(int))
        {
            int stateValue = (int)Convert.ChangeType(value, typeof(int));
            switch (label)
            {
                case "claimed":
                    nodeVisualDriver.SetClaimed(stateValue > 0);
                    break;
                case "kusama_validator_is_active":
                    nodeVisualDriver.SetActive(stateValue == 2);
                    break;
                case "kusama_validator_is_parachain":
                    nodeVisualDriver.SetPara(stateValue > 0);
                    break;
                case "kusama_validator_is_online":
                    nodeVisualDriver.SetOnline(stateValue > 0);
                    break;
                case "kusama_validator_is_selected":
                    nodeVisualDriver.SetSelected(stateValue > 0);
                    break;
            }
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
            HS.Pool.Instance.GetSpawnFromPrefab(stakeEffectFx, source.transform);
        }
    }
}
