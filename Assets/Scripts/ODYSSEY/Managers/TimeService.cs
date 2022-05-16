using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey.Networking;
using Odyssey;


public interface ITimeService
{
    public void StartRunning();
    public void Stop();

    public void SetManualMode(bool manualMode);
    public void SetSkyboxIndex(int idx);
}

// used to handle the ticker and skybox updates;
public class TimeService : MonoBehaviour, ITimeService
{
    private HS.TickerDriver tickerDriver = null;
    private System.DateTimeOffset currentTime;
    public bool manualMode = false;
    public int manualIndex = 0;
    public string topLabel;
    public string bottomLabel;
    private string cachedTopLabel;
    private string cachedBottomLabel;
    private bool _initialUpdate = true;
    public int[] weightedSkyBoxList;
    private int lastHour;
    private readonly WaitForSeconds updateExperienceStatusDelay = new WaitForSeconds(1f);

    Coroutine updateCoroutine = null;

    private bool _isRunning = false;

    void Awake()
    {
        cachedTopLabel = topLabel;
        cachedBottomLabel = bottomLabel;
    }


    public void StartRunning()
    {
        if (_isRunning) return;

        _isRunning = true;
        _initialUpdate = true;

        FindTicker();
        InitSkybox();

        if (updateCoroutine == null) updateCoroutine = StartCoroutine(updateExperienceStatus());
    }

    public void Stop()
    {

        StopCoroutine(updateCoroutine);
        updateCoroutine = null;
        _isRunning = false;
    }

    void InitSkybox()
    {
        int currentSkybox;
        int currentHour;

        currentTime = System.DateTime.Now;
        currentHour = currentTime.Hour;
        currentSkybox = weightedSkyBoxList[currentHour];
        lastHour = currentTime.Hour;

        HS.SkyboxManager.SetSkybox(currentSkybox);
    }

    void FindTicker()
    {
        try
        {
            GameObject ticker = GameObject.FindGameObjectWithTag("Ticker");

            if (ticker != null)
            {
                tickerDriver = ticker.GetComponent<HS.TickerDriver>();
            }

        }
        catch (System.Exception e)
        {
            Logging.Log("[EventTime] Ticker tag not present in project! " + e.Message);
        }

    }

    IEnumerator updateExperienceStatus()
    {

        while (true)
        {
            currentTime = System.DateTime.Now;

            double countDownTimerMinutes = 60 - currentTime.Minute;
            double countDownTimerSeconds = 60 - currentTime.Second;

            int currentHour = currentTime.Hour;

            if (manualMode == false)
            {
                if (currentHour != lastHour)
                {
                    HS.SkyboxManager.SetSkybox(weightedSkyBoxList[currentHour]);
                    lastHour = currentHour;
                }
            }
            else
            {
                if (manualIndex > 9)
                {
                    manualIndex = 0;
                }
                else if (manualIndex < 0)
                {
                    manualIndex = 9;
                }

                HS.SkyboxManager.SetSkybox(manualIndex);
            }

            if (tickerDriver != null && tickerDriver.isSetup)
            {
                tickerDriver.SetTopTicker(currentTime);
                tickerDriver.SetBottomTicker($"00:{countDownTimerMinutes:00}:{countDownTimerSeconds:00}");

                // don't update the ticker labels, unless they have changed
                if (cachedBottomLabel != bottomLabel || cachedTopLabel != topLabel || _initialUpdate == true)
                {
                    _initialUpdate = false;
                    tickerDriver.SetBottomLabel(bottomLabel);
                    tickerDriver.SetTopLabel(topLabel);
                    cachedTopLabel = topLabel;
                    cachedBottomLabel = bottomLabel;
                }

                int secondsToNextHour = (int)countDownTimerMinutes * 60 + (int)countDownTimerSeconds;

                if (secondsToNextHour == 0)
                {
                    tickerDriver.SetBottomTicker("00:00:00");
                }
            }

            yield return updateExperienceStatusDelay;
        }
    }

    public void SetManualMode(bool manualMode)
    {
        this.manualMode = manualMode;
    }

    public void SetSkyboxIndex(int idx)
    {
        this.manualIndex = idx;
    }
}
