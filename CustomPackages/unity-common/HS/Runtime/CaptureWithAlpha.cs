using UnityEngine;
using System.IO;
using System;

[RequireComponent (typeof(Camera))]
public class CaptureWithAlpha : MonoBehaviour
{
    public int UpScale = 4;
    public bool AlphaBackground = true;

    Texture2D Screenshot ()
    {
        var camera = GetComponent<Camera> ();
        int w = camera.pixelWidth * UpScale;
        int h = camera.pixelHeight * UpScale;
        var rt = new RenderTexture (w, h, 32);
        camera.targetTexture = rt;
        var screenShot = new Texture2D (w, h, TextureFormat.ARGB32, false);
        var clearFlags = camera.clearFlags;
        if (AlphaBackground) {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color (0, 0, 0, 0);
        }
        camera.Render ();
        RenderTexture.active = rt;
        screenShot.ReadPixels (new Rect (0, 0, w, h), 0, 0);
        screenShot.Apply ();
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate (rt);
        camera.clearFlags = clearFlags;
        return screenShot;
    }

    [ContextMenu ("Capture Screenshot")]
    public void SaveScreenshot ()
    {
        var path = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
        var filename = "SS-" + DateTime.Now.ToString ("yyyy.MM.dd.HH.mm.ss") + ".png";
        File.WriteAllBytes (Path.Combine (path, filename), Screenshot ().EncodeToPNG ());
    }
}