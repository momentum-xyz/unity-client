using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;
using Cysharp.Threading.Tasks;

public class RewardsBehaviour : MonoBehaviour, IWorldBehaviour, IEffectsTrigger, IStructureState
{

    // Effect Type: 301 - Shower
    // Effect Type: 302 - Distribute reward
    public const int REWARDS_SHOWER_EFFECT_ID = 301;
    public const int REWARDS_DISTRIBUTE_EFFECT_ID = 302;

    [SerializeField] RewardStateVisualsDriver rewardsDriver;
    [SerializeField] GameObject rewardsPrefab;

    #region IWorldBehaviour
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

    }

    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {

    }
    #endregion

    #region IEffectsTrigger
    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int type)
    {

    }

    async UniTask TriggerRewardsEffectAfterDelay(Transform destination, float delay)
    {
        await UniTask.Delay((int)delay);
        HS.Pool.Instance.GetSpawnFromPrefab(rewardsPrefab, destination);
    }

    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type)
    {
        if (type == REWARDS_DISTRIBUTE_EFFECT_ID)
        {
            // Rewards Distribution
            TriggerRewardsEffectAfterDelay(destination.transform, UnityEngine.Random.Range(1000, 6000)).Forget();
        }
    }

    public void TriggerEffect(Vector3 source, int type)
    {

    }

    public void TriggerEffect(GameObject source, int type)
    {
        if (type == REWARDS_SHOWER_EFFECT_ID)
        {
            rewardsDriver.Shower();
        }
    }
    #endregion

    #region IStructureState
    public void SetState<T>(string label, T value)
    {
        if (label == "rewardsamount")
        {
            int v = (int)Convert.ChangeType(value, typeof(int));
            float normalized = Mathf.Clamp01((float)v / 100.0f);
            rewardsDriver.SetRewards(normalized);
        }
    }

    public T GetState<T>(string label)
    {
        return (T)Convert.ChangeType(-1, typeof(T));
    }
    #endregion
}
