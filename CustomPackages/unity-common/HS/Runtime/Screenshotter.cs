using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
	public KeyCode ScreenshotKey = KeyCode.F9;

	void Update()
	{
		if( Input.GetKeyDown( ScreenshotKey ) )
		{
			var file = $"{Application.dataPath}/ScreenShots/Screenshot.png";
			Debug.Log( $"Saving {file}" );
			ScreenCapture.CaptureScreenshot( file );
		}
	}
}
