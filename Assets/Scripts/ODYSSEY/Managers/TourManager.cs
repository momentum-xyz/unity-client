using System;
using Odyssey;
using UnityEngine;

public interface ITourManager
{
    void Start();
    void Stop();
}

public class TourManager : ITourManager, IRequiresContext
{
    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    public void Start()
    {
        var reactBridge = _c.Get<IUnityJSAPI>();
        reactBridge.GoToWaypoint_Event += OnGoToWaypoint;
        reactBridge.CancelGoToWaypoint_Event += OnCancelGoToWaypoint;
    }

    public void Stop()
    {
        var reactBridge = _c.Get<IUnityJSAPI>();
        reactBridge.GoToWaypoint_Event += OnGoToWaypoint;
        reactBridge.CancelGoToWaypoint_Event -= OnCancelGoToWaypoint;
    }

    private void OnGoToWaypoint(Vector3 position, int waypointIndex)
    {
        var teleportSystem = _c.Get<ITeleportSystem>();
        _teleportID = teleportSystem.TeleportToPosition(position, onDone: () =>
            {
                _teleportID = null;
                _c.Get<IUnityToReact>().SendWaypointReached(waypointIndex);
            });
    }
    private void OnCancelGoToWaypoint()
    {
        if (_teleportID != null)
        {
            _c.Get<ITeleportSystem>().StopTeleport(_teleportID);
        }
    }

    IMomentumContext _c;
    Coroutine _teleportID;
}