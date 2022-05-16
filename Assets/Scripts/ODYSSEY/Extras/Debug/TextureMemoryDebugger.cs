using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureMemoryDebugger : MonoBehaviour
{

    public static void DebugMemory()
    {
        long totalTextureMemoryUsage = 0;
        Texture[] texs = Resources.FindObjectsOfTypeAll<Texture>();
        for(var i=0; i < texs.Length; ++i)
        {
            if (!texs[i].name.Contains("downloaded_")) continue;
            long mem = Profiler.GetRuntimeMemorySizeLong(texs[i]);
            Debug.Log(texs[i].name + " => " + texs[i].width + "," + texs[i].height + " "+texs[i].graphicsFormat + " Mem usage: "+mem, texs[i]);
            totalTextureMemoryUsage += mem;
        }
        Debug.Log("Total Texture Memory: " + totalTextureMemoryUsage);
        MemoryManager.ShowMemoryStats();
    }
}
