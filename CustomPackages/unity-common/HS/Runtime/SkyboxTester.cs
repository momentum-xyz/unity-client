using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace HS
{
	public class SkyboxTester : MonoBehaviour
	{
		public KeyCode PreviousKey = KeyCode.LeftBracket;
		public KeyCode NextKey = KeyCode.RightBracket;

		public GameObject TitleMessagePanel;


		void Start()
		{
			DisplayTitle();
		}


		void Update()
		{
			if( Input.GetKeyDown( PreviousKey ) )
			{
				SkyboxManager.Previous();
				DisplayTitle();
			}
			else if( Input.GetKeyDown( NextKey ) )
			{
				SkyboxManager.Next();
				DisplayTitle();
			}
		}

		void DisplayTitle()
		{
			if( TitleMessagePanel )
			{
				var label = TitleMessagePanel.GetComponentsInChildren<TMPro.TMP_Text>(true).Where( elm=>elm.name=="[LABEL]" )?.ToList()[0];
				label.text = "SKYBOX: "+SkyboxManager.CurrentSkyboxName;
				TitleMessagePanel.SetActive( true );
				TitleMessagePanel.GetComponent<Animator>().Rebind();
			}
		}
	}
}