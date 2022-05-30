using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Odyssey;
using Odyssey.Networking;
using System;

public class UnityToReactCallbacks : MonoBehaviour, IRequiresContext
{
    delegate string delegateGetWorldID();
    delegate string delegateGetUserPosition();
    delegate string delegateGetVolumeMode();
    delegate void delegateSetIntState(string guid, string label, int value);
    delegate int delegateGetIntState(string guid, string label);
    delegate void delegateSetStrState(string guid, string label, string value);
    delegate string delegateGetStrState(string guid, string label);
    delegate void delegateTriggerInteractionMsg(int kind, string guid, int flag, string message);

    static IMomentumContext _c;

    private void Awake()
    {
        // Connects C# side callbacks to the callback pointers on the Native side
#if !UNITY_EDITOR && UNITY_WEBGL
        set_callbacks(
            callbackGetWorldID, callbackGetUserPosition, callbackGetVolumeMode, callbackTriggerInteractionMsg
        );

        set_stateCallbacks(
            callbackSetIntState,
            callbackGetIntState,
            callbackSetStrState,
            callbackGetStrState
        );
#endif

    }

    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    void OnDisable()
    {
        _c = null;
    }

    [MonoPInvokeCallback(typeof(delegateGetWorldID))]
    private static string callbackGetWorldID()
    {
        return _c.Get<ISessionData>().WorldID.ToString();
    }

    [MonoPInvokeCallback(typeof(delegateGetUserPosition))]
    private static string callbackGetUserPosition()
    {
        Vector3 playerPosition = _c.Get<ISessionData>().WorldAvatarController.transform.position;
        return playerPosition.ToString();
    }

    [MonoPInvokeCallback(typeof(delegateGetVolumeMode))]
    private static string callbackGetVolumeMode()
    {
        return _c.Get<ISessionData>().MutedSound.ToString();
    }

    [MonoPInvokeCallback(typeof(delegateSetIntState))]
    private static void callbackSetIntState(string guid, string label, int value)
    {
        _c.Get<IWorldObjectsStateManager>().SetState<int>(guid, label, value);
    }

    [MonoPInvokeCallback(typeof(delegateGetIntState))]
    private static int callbackGetIntState(string guid, string label)
    {
        return _c.Get<IWorldObjectsStateManager>().GetState<int>(guid, label);
    }


    [MonoPInvokeCallback(typeof(delegateSetStrState))]
    private static void callbackSetStrState(string guid, string label, string value)
    {
        _c.Get<IWorldObjectsStateManager>().SetState<string>(guid, label, value);
    }

    [MonoPInvokeCallback(typeof(delegateGetStrState))]
    private static string callbackGetStrState(string guid, string label)
    {
        return _c.Get<IWorldObjectsStateManager>().GetState<string>(guid, label);
    }

    [MonoPInvokeCallback(typeof(delegateTriggerInteractionMsg))]
    private static void callbackTriggerInteractionMsg(int kind, string guid, int flag, string message)
    {
        _c.Get<IPosBus>().TriggerInteractionMsg((uint)kind, Guid.Parse(guid), flag, message);
    }


#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void set_callbacks(
    delegateGetWorldID getWorldID,
    delegateGetUserPosition getUserPosition,
    delegateGetVolumeMode getVolumeMode,
    delegateTriggerInteractionMsg triggerInteractionMsg
    );

    [DllImport("__Internal")]
    private static extern void set_stateCallbacks(
        delegateSetIntState setIntState,
        delegateGetIntState getIntState,
        delegateSetStrState setStrState,
        delegateGetStrState getStrState
    );
#endif

}
