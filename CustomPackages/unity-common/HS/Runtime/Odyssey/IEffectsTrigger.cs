using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEffectsTrigger
{
    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int type);
    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type);
    public void TriggerEffect(Vector3 source, int type);
    public void TriggerEffect(GameObject source, int type);

}
