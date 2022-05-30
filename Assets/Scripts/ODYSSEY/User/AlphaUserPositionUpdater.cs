using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using Odyssey;
using Odyssey.Networking;

/// <summary>
/// Send position updates for the current Player/User
/// </summary>
public class AlphaUserPositionUpdater : MonoBehaviour
{
    public float UserPositionUpdateFrequence = 0.5f;
    public float PosBusHearthBeatFrequence = 2.5f;

    private WaitForSeconds userUpdatePositionDelay = null;
    private WaitForSeconds sendHeartbeatDelay = null;

    public event Action<Vector3> PositionUpdated_Event;

    private void Start()
    {
        userUpdatePositionDelay = new WaitForSeconds(UserPositionUpdateFrequence);
        sendHeartbeatDelay = new WaitForSeconds(PosBusHearthBeatFrequence);
    }


    public void StartPositionUpdates()
    {
        StartCoroutine(userUpdatePosition());
        StartCoroutine(sendHeartbeat());
    }

    void OnDestroy()
    {

    }

    // actually updates the position
    IEnumerator userUpdatePosition()
    {
        while (true)
        {
            // don't run if the user hasn't moved
            if (this.transform.hasChanged)
            {
                PositionUpdated_Event?.Invoke(transform.position);
                this.transform.hasChanged = false;

            }

            yield return userUpdatePositionDelay;
        }
    }

    // send a heart beat to posbus every 5 seconds to prevent people from vanishing from the other clients when standing still 
    // The WispManager will remove all Wisps than hasn't been active for 10 seconds
    IEnumerator sendHeartbeat()
    {
        while (true)
        {
            PositionUpdated_Event?.Invoke(transform.position);
            yield return sendHeartbeatDelay;
        }
    }
}
