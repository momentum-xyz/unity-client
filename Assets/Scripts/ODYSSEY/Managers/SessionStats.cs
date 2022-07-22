using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;
using Odyssey;
using Odyssey.Networking;


public interface ISessionStats
{
    public void AddTime(string label);
    public void StartSession();

    public void FlushSession(string userId, string sessionId, string worldId);
    public void StopDebugAndStart(string label, Stopwatch watch);
}

public class SessionStats : ISessionStats
{
    public Stopwatch stopWatch;

    public StringBuilder fullStats;

    private bool firstRow = true;

    private StringBuilder tempSB;

    public void StartSession()
    {
        stopWatch = new Stopwatch();
        tempSB = new StringBuilder();
        fullStats = new StringBuilder();
        fullStats.Append("{\"events\":[");
    }

    public void AddTime(string label)
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

    public void FlushSession(string userId, string sessionId, string worldId)
    {
        fullStats.Append("],\"world\":\"");
        fullStats.Append(worldId);
        fullStats.Append("\"}");
        Debug.Log(fullStats.ToString());

        stopWatch.Stop();
    }

    public void StopDebugAndStart(string label, Stopwatch watch)
    {
        watch.Stop();
        Debug.Log("[" + label + "] took " + ((float)watch.ElapsedMilliseconds / 1000.0f));
        watch.Reset();
        watch.Start();

    }
}
