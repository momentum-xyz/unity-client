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
    private Vector3 positionCache;                   // used to determine the last broadcast position of this user
    private int wispX;                               // cache so we aren't working this out on the fly
    private int wispY;                               // cache so we aren't working this out on the fly
    private int wispZ;                               // cache so we aren't working this out on the fly
    public Vector3 userPosition;

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
        int newWispX;
        int newWispY;
        int newWispZ;

        while (true)
        {
            // don't run if the user hasn't moved
            if (this.transform.position != positionCache)
            {
                userPosition = this.transform.position;

                newWispX = (int)Mathf.Round(positionCache.x);
                newWispY = (int)Mathf.Round(positionCache.y);
                newWispZ = (int)Mathf.Round(positionCache.z);

                // dampens tiny movements - we don't need to send
                if (newWispX != wispX || newWispY != wispY || newWispZ != wispZ)
                {
                    wispX = newWispX;
                    wispY = newWispY;
                    wispZ = newWispZ;

                    PositionUpdated_Event?.Invoke(transform.position);
                }

                positionCache = this.transform.position;
            }

            // update the position cache incase we missed the movement
            positionCache = this.transform.position;

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
