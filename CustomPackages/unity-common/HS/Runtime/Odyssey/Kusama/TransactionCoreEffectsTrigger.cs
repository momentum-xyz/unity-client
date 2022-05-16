using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionCoreEffectsTrigger : MonoBehaviour, IEffectsTrigger
{
    public Transform randomOrigin;

    public void TriggerEffect(Vector3 source, int effectType)
    {

    }

    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int effectType)
    {
        // reserve effectType from 0 to 4 for this type of effect (this is very random at this point)
        if (effectType > 0 && effectType < 5)
        {

            Vector3 sourcePosition = source;

            if(randomOrigin != null)
            {
                sourcePosition = randomOrigin.position + UnityEngine.Random.insideUnitSphere * 100.0f;
            }

            HS.ParticleTransactionCore.Add(sourcePosition, destination, UnityEngine.Random.Range(1, 10), effectType);
        }

    }

    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type)
    {

    }

    public void TriggerEffect(GameObject source, int type)
    {

    }
}
