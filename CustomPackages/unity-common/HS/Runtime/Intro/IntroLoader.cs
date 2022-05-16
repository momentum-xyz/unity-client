using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HS
{
	public class IntroLoader : MonoBehaviour
	{
		public string ActualIntroScene = "ArrivalSetupScene";
		public string DebugStuffToRemove = "[TESTING]";


		IEnumerator Start()
		{
			// WARNING: Playing with fire here, the structure of the loaded scene
			// NEEDS to take into account that this wrapper scene might be changing the ShortVersion bool.
			DontDestroyOnLoad( gameObject );
			SceneManager.LoadScene( ActualIntroScene );
			yield return null;
			var manager = FindObjectOfType<ArrivalSequenceManager>();
			manager.ShortVersion = true;
			var testStuff = GameObject.Find( DebugStuffToRemove );
			Destroy( testStuff );
			Destroy( gameObject );
			yield break;
		}


		void OnGUI()
		{
			GUI.color = Color.black;
			GUI.DrawTexture( new Rect(0, 0, Screen.width, Screen.height), null );
		}
	}
}