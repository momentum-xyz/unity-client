using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class OdysseyEditorUtils
{
    [MenuItem("ODYSSEY/Load MainScene")]
    public static void LoadMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
    }

    [MenuItem("ODYSSEY/Clear Addressables Cache", false, 50)]
    public static void ClearAddressablesCache()
    {
        Debug.Log("clear cache at " + Application.persistentDataPath);
        var list = Directory.GetDirectories(Application.persistentDataPath);

        foreach (var item in list)
        {
            Debug.Log("Delete" + " " + item);
            Directory.Delete(item, true);
        }

        Caching.ClearCache();
    }
}
