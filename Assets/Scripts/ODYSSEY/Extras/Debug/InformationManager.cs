using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Odyssey;

public class InformationManager : MonoBehaviour, IRequiresContext
{
#if !UNITY_EDITOR && UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void DownloadFile(string textString, string fileNamePtr);

	//Browser information functions
	[DllImport("__Internal")]
	private static extern string GetBrowserName();
	[DllImport("__Internal")]
	private static extern string GetBrowserInfo();
#endif

    [SerializeField]
    GameObject DebugInfo;


    public bool showStats = true;

    IMomentumContext _c;

    void Start()
    {
        _informationCollectors = DebugInfo.GetComponentsInChildren<IInformationCollector>();
    }

    public void Init(IMomentumContext context)
    {
        this._c = context;
    }

    private string CollectAllInformation()
    {
        string toReturn = string.Empty;
        for (int i = 0; i < _informationCollectors.Length; i++)
        {
            var icollector = _informationCollectors[i];
            toReturn += icollector.GetInfo();
        }

        toReturn += "\n\n";
#if !UNITY_EDITOR && UNITY_WEBGL
		toReturn += "Browsername: " + GetBrowserName() + "\n";
		toReturn += GetBrowserInfo() + "\n";
#endif
        toReturn += "User ID : " + _c.Get<ISessionData>().UserID + "\n";
        toReturn += "Unity version : " + Globals.UNITYCLIENTVERSION + "\n";
        toReturn += "Paused : " + _c.Get<ISessionData>().AppPaused;


        return toReturn;
    }

    private string GenerateFileName()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
		string toReturn = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + SystemInfo.operatingSystem + " " + GetBrowserName();
#else
        string toReturn = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + SystemInfo.operatingSystem;
#endif

        return toReturn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {

            DebugInfo.SetActive(true);

#if !UNITY_EDITOR && UNITY_WEBGL
			DownloadFile(CollectAllInformation(), GenerateFileName());
#else
            print(CollectAllInformation());
#endif

        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            showStats = !showStats;
            Debug.Log("Debug info : User ID " + _c.Get<ISessionData>().UserID);    // not using odddebug here as this should be always visible
            DebugInfo.SetActive(showStats);
        }
    }

    IInformationCollector[] _informationCollectors;
}
