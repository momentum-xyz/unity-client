using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using UnityEngine.Profiling;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Unity.Profiling.LowLevel.Unsafe;
using System.Text;
using Unity.Profiling;

public class MemoryManager : MonoBehaviour, IRequiresContext
{
    public static MemoryManager i;

    static float TIME_BETWEEN_MEMORY_CHECK_IN_SEC = 60.0f;
    static float TIME_BETWEEN_STRUCTURE_NOT_VISITED_CHECK_IN_SEC = 10.0f;
    static float TIME_BETWEEN_GARBAGE_CLEAN = 30.0f;
    static float TIME_UNTIL_TEXTURES_CAN_BE_UNLOADED = 60.0f * 15.0f; // if a structure has not been visited for that time, it's textures will be unloaded!
    static long MEM_ALERT_AT = 400000000; // 400MB - 

    public static Action<long> LowMemory_Event;

    public Texture EmptyTexture
    {
        get
        {
            if (_emptyTexture == null)
            {
                _emptyTexture = Resources.Load<Texture>("Textures/black") as Texture;
            }

            return _emptyTexture;
        }
        set { }
    }
    private Texture _emptyTexture;

    private DateTime lastOutOfMemorySignal = DateTime.Now;
    private int numLowMemorySignals = 0;

    private static long lastTextureMemoryUsage = -1;

    IMomentumContext _c;

    public void Init(IMomentumContext context)
    {
        this._c = context;
    }

    public void Dispose()
    {

    }

    void Awake()
    {
        i = this;
    }

    private void Start()
    {
        StartCoroutine(CheckForTextureMemoryUsage());
        StartCoroutine(UnloadTexturesForNotVisitedStructuresRunner());
        StartCoroutine(Clean());

        LowMemory_Event += OnLowMemory;

        _c.Get<IReactBridge>().PauseUnity_Event += OnUnityClientPaused;
    }

    private void OnDestroy()
    {
        LowMemory_Event -= OnLowMemory;

        _c.Get<IReactBridge>().PauseUnity_Event -= OnUnityClientPaused;
    }

    /// <summary>
    /// Clean the Garbage while the Unity client is Paused, so we don't see the jitter it might cause
    /// </summary>
    void OnUnityClientPaused()
    {
        CleanGarbage();
    }

    public static void ShowMemoryStats()
    {
        string s = "";
        s += "------ MEMORY STATS ------\n";
        s += "Total Graphics Memory: " + SystemInfo.graphicsMemorySize + "\n";
        s += "Total Memory: " + SystemInfo.systemMemorySize + "\n";

        Debug.Log(s);

        Debug.Log("LAST MEMORY USAGE: " + lastTextureMemoryUsage);
        //  EnumerateProfilerStats();
    }

    void OnLowMemory(long memoryUsage)
    {
        Logging.Log("[MemoryManager] LOW TEXTURE MEMORY!");

        DateTime currenTime = DateTime.Now;

        numLowMemorySignals++;

        if (currenTime.Subtract(lastOutOfMemorySignal).TotalSeconds < 30.0f)
        {
            //Logging.Log("[MemoryManager] Receiving LOW TEXTURE MEMORY too often!");
            return;
        }

        lastOutOfMemorySignal = DateTime.Now;

        float cleanNotVisitedForTime = 120.0f;
        // If we have received more numLowMemorySignals than 1, that means that 
        // the actions we are taking does not release enough memory, so we need to be
        // more aggressive so we clean all structures that has not been visited for the last 30 seconds
        // in the other case we clean all structures that hasn't been visited for 2 minutes
        if (numLowMemorySignals > 1)
        {
            cleanNotVisitedForTime = 30.0f;
        }

        // if we are 1.5 times over the the mem limit, be even more aggressive!
        if ((float)memoryUsage / (float)MEM_ALERT_AT > 1.5f)
        {
            Logging.LogError("[MemoryManager] We are 1.5 times over the memory usage limit!");
            cleanNotVisitedForTime = 15.0f;
        }

        UnloadTexturesForNotVisitedStructures(cleanNotVisitedForTime);

        numLowMemorySignals = 0;
    }


    public void ForceUnloadAllStructuresTextures()
    {
        Debug.Log("CLEARING ALL TEXTURES!");
        foreach (KeyValuePair<Guid, WorldObject> wo in _c.Get<IWorldData>().WorldHierarchy)
        {
            _c.Get<ITextureService>().UnloadAllTexturesForObject(wo.Value);
        }

        CleanGarbage();
    }

    /// <summary>
    /// Goes through all assets with 0 references and frees up the memory
    /// </summary>
    public static void CleanGarbage()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    IEnumerator Clean()
    {
        while (true)
        {
            yield return new WaitForSeconds(TIME_BETWEEN_GARBAGE_CLEAN);
            CleanGarbage();
        }
    }

    IEnumerator CheckForTextureMemoryUsage()
    {
        while (true)
        {
            yield return new WaitForSeconds(TIME_BETWEEN_MEMORY_CHECK_IN_SEC);

            long totalTextureMemoryUsage = 0;
            long estimatedMemoryUsage = 0;

            Texture[] texs = Resources.FindObjectsOfTypeAll<Texture>();
            for (var i = 0; i < texs.Length; ++i)
            {
                if (!texs[i].name.Contains("downloaded_")) continue;

                // !!! GetRuntimeMemorySize won't show GPU memory usage on WebGL (when Development build is disabled)
                long mem = 0;

                if (!Debug.isDebugBuild && Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    mem = EstimateMemoryUsageForTexture(texs[i]);
                    estimatedMemoryUsage += mem;
                }
                else
                {
                    mem = Profiler.GetRuntimeMemorySizeLong(texs[i]);
                    totalTextureMemoryUsage += mem;
                    estimatedMemoryUsage = totalTextureMemoryUsage;
                }

                //Debug.Log(texs[i].name + " => " + texs[i].width + "," + texs[i].height + " " + texs[i].graphicsFormat + " Mem usage: " + mem, texs[i]);

            }

            //Debug.Log("Total Texture Memory: " + totalTextureMemoryUsage+ " estimated: "+ estimatedMemoryUsage);

            lastTextureMemoryUsage = totalTextureMemoryUsage > 0 ? totalTextureMemoryUsage : estimatedMemoryUsage;

            if (totalTextureMemoryUsage > MEM_ALERT_AT || estimatedMemoryUsage > MEM_ALERT_AT)
            {
                LowMemory_Event?.Invoke(totalTextureMemoryUsage > 0 ? totalTextureMemoryUsage : estimatedMemoryUsage);
            }



        }

    }

    long EstimateMemoryUsageForTexture(Texture t)
    {
        return (long)t.width * t.height * 4;
    }

    public void UnloadTexturesForNotVisitedStructures(float notVisitedForTime)
    {
        // Logging.Log("[MemoryManager] Trying to release textures of unvisited structures for the last "+notVisitedForTime+" seconds.");

        float currentTime = Time.fixedTime;

        bool texturesHaveBeenRemoved = false;

        foreach (KeyValuePair<Guid, WorldObject> worldObject in _c.Get<IWorldData>().WorldHierarchy)
        {

            AlphaStructureDriver alphaStructureDriver = worldObject.Value.GetStructureDriver();

            if (alphaStructureDriver == null) continue;

            WorldObject wo = worldObject.Value;

            if (alphaStructureDriver.neverUnload) continue;

            if (currentTime - alphaStructureDriver.lastVisit > notVisitedForTime)
            {

                // check if the structure is close to the user at this point and don't release it's textures
                bool IsReallyCloseToUser = false;

                if (_c.Get<ISessionData>().AvatarCamera != null && wo.GO != null)
                {
                    float distance = Vector3.SqrMagnitude(_c.Get<ISessionData>().AvatarCamera.transform.position - wo.GO.transform.position);
                    IsReallyCloseToUser = distance < 10000.0f;
                }

                if (IsReallyCloseToUser)
                {
                    continue;
                }

                // don't release the meme, if we have enough memory
                bool skipMeme = lastTextureMemoryUsage < MEM_ALERT_AT ? true : false;

                _c.Get<ITextureService>().UnloadAllTexturesForObject(worldObject.Value, skipMeme);
                texturesHaveBeenRemoved = true;
            }
        }

        if (texturesHaveBeenRemoved == true)
        {
            CleanGarbage();
        }
    }

    // periodic texture unload for unvisited platforms
    IEnumerator UnloadTexturesForNotVisitedStructuresRunner()
    {
        while (true)
        {
            if (!_c.Get<ISessionData>().WorldIsTicking)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(TIME_BETWEEN_STRUCTURE_NOT_VISITED_CHECK_IN_SEC);

            UnloadTexturesForNotVisitedStructures(TIME_UNTIL_TEXTURES_CAN_BE_UNLOADED);


        }
    }
}
