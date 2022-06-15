using UnityEngine;
using System.Runtime.InteropServices;

public interface IReactAPI
{
    public void SendMomentumLoadedToReact();
    public void SendReadyToTeleportToReact();
    public void ExterminateUnity();
    public string GetGraphicCardFromBrowser();
    public string GetBrowser();
    public void SendTeamPlasmaClick(string teamID);
    public void SendClick(string guid, string label);

    public void SendProfileClickEvent(string userID, string position);
    public void SendWaypointReached(int waypointIndex);

    public void RelayNotificationSimple(int kind, int flag, string message);
    public void RelayRelayMessage(string target, string message);
    public void SendPosBusConnected();

}

public class ReactAPI : IReactAPI
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void ProfileClickEvent(string userID);

    [DllImport("__Internal")]
    private static extern void Screen1ClickEvent(string teamID);

    [DllImport("__Internal")]
    private static extern void Screen2ClickEvent(string teamID);

    [DllImport("__Internal")]
    private static extern void Screen3ClickEvent(string teamID);

    [DllImport("__Internal")]
    private static extern void TeamPlasmaClickEvent(string teamID);
    
    [DllImport("__Internal")]
    private static extern void SendClickEvent(string msg);

    [DllImport("__Internal")]
    private static extern void SendExterminateUnityRequest();

    [DllImport("__Internal")]
    private static extern void SendReadyForTeleport();

    [DllImport("__Internal")]
    private static extern string GetGraphicCard();

    [DllImport("__Internal")]
    private static extern string GetBrowserName();

    [DllImport("__Internal")]
    private static extern void MomentumLoaded();

    [DllImport("__Internal")]
    private static extern void WaypointReached(int waypoint);

    [DllImport("__Internal")]
    private static extern void SimpleNotification(int kind, int flag, string message);

    [DllImport("__Internal")]
    private static extern void RelayMessage(string target, string message);

    [DllImport("__Internal")]
    private static extern void PosBusConnected();


#endif

    public void SendMomentumLoadedToReact()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        MomentumLoaded();
#endif
    }
    public void SendReadyToTeleportToReact()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SendReadyForTeleport();
#endif
    }
    public void ExterminateUnity()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        SendExterminateUnityRequest();
#endif
    }
    public string GetGraphicCardFromBrowser()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return GetGraphicCard();
#else
        return "Non WebGL Platform";
#endif
    }

    public string GetBrowser()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        return GetBrowserName();
#else
        return "Non WebGL Platform";
#endif
    }
    public void SendTeamPlasmaClick(string teamID)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            // OddDebug.Log("TeamPlasmaClick sent " + teamID.ToString(), 2);
            TeamPlasmaClickEvent(teamID);
#endif
    }
    public void SendClick(string guid, string label)
    {
        Debug.Log("Sending click: " + label + " to " + guid);
#if !UNITY_EDITOR && UNITY_WEBGL
        SendClickEvent(label+"|"+guid);
#endif
    }

    public void SendProfileClickEvent(string userID, string position)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        string completeMessage = "";
        // OddDebug.Log("clicked on profile : " + userID, 3);

        position = position.Replace("(", string.Empty);
        position = position.Replace(")", string.Empty);
        position = position.Replace(" ", string.Empty);
        position = position.Replace(',', ':');

        completeMessage = userID + "|" + position;

        ProfileClickEvent(completeMessage);        
#endif
    }
    public void SendWaypointReached(int waypointIndex)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WaypointReached(waypointIndex);
#endif
        Debug.Log("[UnityToReact] SendWaypointReached " + waypointIndex);
    }

    public void RelayNotificationSimple(int kind, int flag, string message)
    {
        Debug.Log("Relaying: " + kind + " / " + flag + " / " + message);
#if !UNITY_EDITOR && UNITY_WEBGL
        SimpleNotification(kind, flag, message);
#endif
    }

    public void RelayRelayMessage(string target, string message)
    {
        Debug.Log("Relaying: " + target + " / " + message);
#if !UNITY_EDITOR && UNITY_WEBGL
        RelayMessage(target, message);
#endif
    }

    public void SendPosBusConnected()
    {
        Debug.Log("[UnityToReact] PosBusConnected");
#if !UNITY_EDITOR && UNITY_WEBGL
        PosBusConnected();
#endif
    }
}
