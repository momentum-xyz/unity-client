using System.Collections;
using UnityEngine;
using System;
using Odyssey;

public interface ITeleportSystem
{
    void OnTeleportToUser(string guid);
    void OnTeleportToSpace(string guid);
    Coroutine TeleportToPosition(Vector3 position, Action onDone);
    void StopTeleport(Coroutine teleportID);
}

public class TeleportSystem : MonoBehaviour, ITeleportSystem, IRequiresContext
{
    IMomentumContext _c;

    private bool IsTeleporting = false;

    private float teleportTime = 3.0f;
    private float teleportSpinTime = 0.4f;
    private float stopDistance = 12.0f;

    void Start()
    {

    }

    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    void OnEnable()
    {
        _c.Get<IReactBridge>().TeleportToSpace_Event += OnTeleportToSpace;
        _c.Get<IReactBridge>().TeleportToPosition_Event += OnTeleportToPosition;
        _c.Get<IReactBridge>().TeleportToUser_Event += OnTeleportToUser;
    }

    public void OnDisable()
    {
        _c.Get<IReactBridge>().TeleportToSpace_Event -= OnTeleportToSpace;
        _c.Get<IReactBridge>().TeleportToPosition_Event -= OnTeleportToPosition;
        _c.Get<IReactBridge>().TeleportToUser_Event -= OnTeleportToUser;
    }

    public void OnTeleportToUser(string guid)
    {
        WispData data = _c.Get<IWispManager>().GetWispDataForGuid(new Guid(guid));

        if (data == null)
        {
            Logging.Log("[TeleportSystem] could not find wisp with id: " + guid);
            return;
        }

        Vector3 position = data.currentPosition;

        FlyToDestination(position, true);

    }
    public void OnTeleportToSpace(string spaceUid)
    {
        Guid guid = Guid.Parse(spaceUid);

        WorldObject wo = _c.Get<IWorldData>().Get(guid);

        if (wo == null)
        {
            Logging.LogError("[TeleportSystem] World Object missing for guid: " + guid.ToString());
            return;
        };



        Vector3 teleportPosition = wo.WorldPosition();


        AlphaStructureDriver structurDriver = wo.GetStructureDriver();
        bool skipDefaultOffset = false;

        if (structurDriver != null)
        {
            if (structurDriver.teleportPoint != null)
            {
                teleportPosition = structurDriver.teleportPoint.position;
                skipDefaultOffset = true;
            }
        }

        FlyToPlatform(wo.GO, wo.WorldPosition(), teleportPosition, skipDefaultOffset);


    }

    public Coroutine TeleportToPosition(Vector3 waypoint, Action onDone)
    {
        var avatarTransform = _c.Get<ISessionData>().WorldAvatarController.transform;
        return StartCoroutine(FlyToWithLookAtDestination(avatarTransform, waypoint, teleportTime, onDone));
    }

    public void StopTeleport(Coroutine teleportID)
    {
        StopCoroutine(teleportID);
    }

    private void OnTeleportToPosition(Vector3 destination)
    {
        FlyToDestination(destination, false);
    }

    IEnumerator FlyTo(Transform playerTransform, Vector3 destination, float timeToTeleport)
    {
        var currentPosition = playerTransform.position;

        float moveTime = 0f;

        Vector3 startPosition = playerTransform.position;

        while (Vector3.Distance(destination, playerTransform.position) > stopDistance)
        {
            // Give back the control to the user, if he presses a key
            if (_c.Get<HS.IThirdPersonController>().IsControlling)
            {
                break;
            }

            moveTime += Time.deltaTime;
            playerTransform.position = Vector3.Lerp(startPosition, destination, moveTime / teleportTime);

            yield return null;
        }


        IsTeleporting = false;

        yield return null;
    }

    /// <summary>
    /// Sends the user flying to a provided destination (Vector3)
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="rotateAtDestination"></param>
    private void FlyToDestination(Vector3 destination, bool rotateAtDestination = true)
    {
        if (_c.Get<ISessionData>().WorldAvatarController == null)
        {
            Logging.LogError("[TeleportSystem] No current Avatar Controller");
            return;
        }

        if (IsTeleporting) return;

        IsTeleporting = true;

        if (rotateAtDestination == true)
        {
            StartCoroutine(LookAtDesination(_c.Get<ISessionData>().WorldAvatarController.transform, destination, teleportSpinTime));
        }

        StartCoroutine(FlyTo(_c.Get<ISessionData>().WorldAvatarController.transform, destination, teleportTime));
    }


    IEnumerator LookAtDesination(Transform playerTransform, Vector3 destination, float timeToLookAt)
    {
        var currentRotation = playerTransform.rotation;
        float time = 0f;

        var targetRotation = Quaternion.LookRotation(destination - playerTransform.position);

        while (time < 1f)
        {
            // Give back the control to the user, if he presses a key
            if (_c.Get<HS.IThirdPersonController>().IsControlling)
            {
                break;
            }

            time += Time.deltaTime / timeToLookAt;
            playerTransform.rotation = Quaternion.Lerp(currentRotation, targetRotation, time);
            yield return null;
        }
    }

    private void FlyToPlatform(GameObject platform, Vector3 lookAtPosition, Vector3 finalDestination, bool skipDefaultOffset = false)
    {
        if (_c.Get<ISessionData>().WorldAvatarController == null)
        {
            Logging.LogError("[TeleportSystem] No current Avatar Controller");
            return;
        }

        if (IsTeleporting) return;

        IsTeleporting = true;

        StartCoroutine(LookAtDesination(_c.Get<ISessionData>().WorldAvatarController.transform, finalDestination, teleportSpinTime));
        StartCoroutine(FlyToWithRotation(platform, lookAtPosition, finalDestination, teleportTime, skipDefaultOffset));

    }

    IEnumerator FlyToWithRotation(GameObject platform, Vector3 lookAtPosition, Vector3 finalDestination, float timeToTeleport, bool skipDefaultOffset = false)
    {
        var avatarController = _c.Get<ISessionData>().WorldAvatarController;
        if (avatarController == null)
        {
            Logging.LogError("[TeleportSystem] No current Avatar Controller");
            yield break;
        }

        float moveTime = 0f;

        Vector3 startPosition = avatarController.transform.position;
        Vector3 destination = finalDestination + (skipDefaultOffset ? Vector3.zero : ((platform.transform.forward * stopDistance) + new Vector3(0, 5.0f, 0)));

        bool cancelRotation = false;
        while (Vector3.Distance(destination, avatarController.transform.position) > 1)
        {
            // Give back the control to the user, if he presses a key
            if (_c.Get<HS.IThirdPersonController>().IsControlling)
            {
                cancelRotation = true;
                break;
            }

            moveTime += Time.deltaTime;
            avatarController.transform.position = Vector3.Lerp(startPosition, destination, moveTime / teleportTime);
            yield return null;
        }

        if (!cancelRotation)
        {
            StartCoroutine(LookAtDesination(avatarController.transform, lookAtPosition, teleportSpinTime));
        }
        IsTeleporting = false;

        yield return null;
    }

    IEnumerator FlyToWithLookAtDestination(Transform playerTransform, Vector3 destination, float timeToTeleport, Action onDone = null)
    {
        Vector3 startPosition = playerTransform.position;
        var currentRotation = playerTransform.rotation;
        var targetRotation = Quaternion.LookRotation(destination - playerTransform.position);
        float time = 0f;
        while (Vector3.Distance(destination, playerTransform.position) > stopDistance)
        {
            time += Time.deltaTime;
            playerTransform.position = Vector3.Lerp(startPosition, destination, time / teleportTime);
            playerTransform.rotation = Quaternion.Lerp(currentRotation, targetRotation, time);

            yield return null;
        }

        IsTeleporting = false;

        yield return null;

        onDone?.Invoke();
    }
}
