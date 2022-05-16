using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Odyssey;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    private static LoadingSceneManager Instance = null;
    public static string mainSceneName = "MainScene";
    private Scene _loadingScene;

    IEnumerator Start()
    {
        Logging.Log("Loading Momentum...");

        _loadingScene = SceneManager.GetActiveScene();

        AsyncOperation loading = SceneManager.LoadSceneAsync(mainSceneName, LoadSceneMode.Additive);

        yield return null;
    }

    // SceneManager.UnloadSceneAsync(_loadingScene);

    private void Awake()
    {
        Instance = this;

    }

    private void OnDestroy()
    {

    }


}
