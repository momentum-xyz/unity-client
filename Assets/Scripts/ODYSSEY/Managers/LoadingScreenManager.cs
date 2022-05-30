using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Odyssey;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public interface ILoadingScreenManager
{
    public void SetLoading(bool isEnabled, bool noAnimation = false);
    public void SetPaused(bool isPaused);
}

public class LoadingScreenManager : MonoBehaviour, IRequiresContext, ILoadingScreenManager
{

    public Canvas loadingCanvas;
    public CanvasGroup loadingCanvasGroup;
    public GameObject displayScreenshotContainer;

    public RawImage pausedRawImage;

    private Texture2D screenshotTexture;
    private bool makeScreenshot = false;

    private IMomentumContext _c;

    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    void Awake()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDestroy()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }


    public void SetLoading(bool isEnabled, bool noAnimation = false)
    {
        if (isEnabled)
        {
            StartCoroutine(FadeIn(1.0f, noAnimation));
        }
        else
        {
            StartCoroutine(FadeOut(1.0f, noAnimation));
        }
    }

    public IEnumerator FadeOut(float seconds, bool noAnimation = false)
    {
        if (loadingCanvas == null)
        {
            yield break;
        }

        if (noAnimation)
        {
            loadingCanvasGroup.alpha = 0.0f;
            loadingCanvas.enabled = false;
            yield break;
        }

        float offset = seconds / 30.0f;
        for (var t = 1.0f; t >= 0.0f; t -= offset)
        {
            loadingCanvasGroup.alpha = t;
            yield return null;
        }

        loadingCanvasGroup.alpha = 0.0f;
        loadingCanvas.enabled = false;

    }

    public IEnumerator FadeIn(float seconds, bool noAnimation = false)
    {
        if (loadingCanvas == null)
        {
            yield break;
        }

        if (noAnimation)
        {
            loadingCanvasGroup.alpha = 1.0f;
            loadingCanvas.enabled = true;
            yield break;
        }

        float offset = seconds / 30.0f;
        loadingCanvas.enabled = true;
        loadingCanvasGroup.alpha = 0.0f;

        for (var t = 0.0f; t < 1.0f; t += offset)
        {
            loadingCanvasGroup.alpha = t;
            yield return null;
        }

        loadingCanvasGroup.alpha = 1.0f;

    }

    public void SetPaused(bool isPaused)
    {
        Debug.Log("Paused: " + isPaused);

        if (isPaused)
        {
            StartCoroutine(UpdateRawImageWithScreenshot(pausedRawImage));

        }
        else
        {

            foreach (var obj in _c.Get<IWorldData>().WorldHierarchy.Values)
            {
                if (obj.GO != null)
                {
                    obj.GO.SetActive(true);
                }
            }

            pausedRawImage.texture = null;
            pausedRawImage.enabled = false;
            pausedRawImage.gameObject.SetActive(false);

            Destroy(screenshotTexture);

            displayScreenshotContainer.SetActive(false);
            _c.Get<ISessionData>().AvatarCamera.enabled = true;

            Time.timeScale = 1.0f;
        }
    }



    IEnumerator UpdateRawImageWithScreenshot(RawImage img)
    {
        // Hide all infoUI elements
        _c.Get<IInfoUIDriver>().HideAll();

        // Wait few frames so infoUI is actually hidden/canvas updated
        yield return new WaitForSeconds(0.1f);

        // trigger screenshot when the camera rendering ended
        makeScreenshot = true;

    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (!makeScreenshot) return;

        // read an RGB texture from screen
        int width = Screen.width;
        int height = Screen.height;

        screenshotTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshotTexture.Apply();

        // Set it to the RawImage
        pausedRawImage.texture = screenshotTexture;

        // Disable rendering
        _c.Get<ISessionData>().AvatarCamera.enabled = false;

        // Disable all Objects in the world

        foreach (var obj in _c.Get<IWorldData>().WorldHierarchy.Values)
        {
            if (obj.GO != null)
            {
                obj.GO.SetActive(false);
            }
        }

        Time.timeScale = 0.0f;

        // Enable the Canvas where we display the screenshot

        displayScreenshotContainer.SetActive(true);
        pausedRawImage.enabled = true;
        pausedRawImage.gameObject.SetActive(true);

        makeScreenshot = false;

    }

}
