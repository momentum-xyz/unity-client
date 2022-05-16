using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HS
{
	public class SceneSwitcher : MonoBehaviour
	{
		public KeyCode SwitchKey = KeyCode.Space;


		void Update()
		{
			if (Input.GetKeyDown(SwitchKey))
			{
				SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
			}
		}

	}
}