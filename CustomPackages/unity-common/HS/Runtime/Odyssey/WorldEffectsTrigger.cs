using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEffectsTrigger : MonoBehaviour, IEffectsTrigger
{
    const int HIGHFIVE_EFFECT_TYPE = 1001;
    const int WOW_EFFECT_TYPE = 1002;

    // !!! Reference all Effects settings here, so their assets will be included with the Addressable bundle
    public ScriptableObject[] effectSettings;

    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int type)
    {
        // Handle Highfives
        if (type == HIGHFIVE_EFFECT_TYPE)
        {
            HS.HighFiveEvent.Create(source, destination);
        }
    }

    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type)
    {

    }

    public void TriggerEffect(Vector3 source, int type)
    {

    }

    public void TriggerEffect(GameObject source, int type)
    {
        // Handle Wow effect
        if (type == WOW_EFFECT_TYPE)
        {
            HS.GetAWowEvent.Create(source);
        }
    }
}
