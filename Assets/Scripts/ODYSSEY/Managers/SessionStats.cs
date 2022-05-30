using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;
using Odyssey;
using Odyssey.Networking;

public class SessionStats : MonoBehaviour
{
    public static Stopwatch stopWatch;

    public static StringBuilder fullStats;

    private static bool firstRow = true;

    private static StringBuilder tempSB;
    public static void StartSession()
    {
        stopWatch = new Stopwatch();
        tempSB = new StringBuilder();
        fullStats = new StringBuilder();
        fullStats.Append("{\"events\":[");
    }

    public static void AddTime(string label)
    {
        stopWatch.Stop();

        long ellapsedMs = stopWatch.ElapsedMilliseconds;
        float unityTime = Time.time;

        tempSB.Clear();
        if (!firstRow) tempSB.Append(",");
        tempSB.Append("{\"");
        tempSB.Append(label);
        tempSB.Append("\":{");
        tempSB.Append("\"ellapsed\":");
        tempSB.Append(((float)ellapsedMs / 1000.0f).ToString());
        tempSB.Append(",\"unityTime\":");
        tempSB.Append(unityTime);
        tempSB.Append("}}");

        fullStats.Append(tempSB);
        stopWatch.Reset();
        stopWatch.Start();

        firstRow = false;
    }

    public static void FlushSession(string userId, string sessionId, string worldId)
    {
        fullStats.Append("],\"world\":\"");
        fullStats.Append(worldId);
        fullStats.Append("\"}");
        Debug.Log(fullStats.ToString());

        stopWatch.Stop();
    }

    public static void StopDebugAndStart(string label, Stopwatch watch)
    {
        watch.Stop();
        Debug.Log("[" + label + "] took " + ((float)watch.ElapsedMilliseconds / 1000.0f));
        watch.Reset();
        watch.Start();

    }
}
