using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour, IEffectsTrigger, IWorldBehaviour, IStructureState
{
    HS.BlockStateVisualsDriver blockStateDriver;

    void Start()
    {
        blockStateDriver = GetComponent<HS.BlockStateVisualsDriver>();
    }
    // these are random for now!
    // type: 10 - Attach To Node
    // type: 11 - Validate

    public void TriggerBridgeEffect(Vector3 source, Vector3 destination, int type)
    {

    }

    public void TriggerBridgeEffect(GameObject source, GameObject destination, int type)
    {
        if (blockStateDriver == null) return;

        if (type == 10)
        {
            blockStateDriver.AttachNode(destination);
        }
    }

    public void TriggerEffect(Vector3 source, int type)
    {

    }

    public void TriggerEffect(GameObject source, int type)
    {
        if (blockStateDriver == null) return;

        if (type == 11)
        {
            var blockStateDriver = GetComponent<HS.BlockStateVisualsDriver>();
            StartCoroutine(blockStateDriver.Validate());
        }
    }

    public void InitBehaviour()
    {
    
    }

    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter)
    {
   
    }

    public void UpdateLOD(int lodLevel)
    {
     
    }

    public void UpdateBehaviour(float dt)
    {
       
    }

    public void FixedUpdateBehaviour(float dt)
    {
     
    }

    public void SetState<T>(string label, T value)
    {

    }

    public T GetState<T>(string label)
    {
        return (T)Convert.ChangeType(-1, typeof(T));
    }
}
