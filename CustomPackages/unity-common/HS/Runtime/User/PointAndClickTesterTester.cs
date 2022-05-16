using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class PointAndClickTesterTester : MonoBehaviour
	{
		void Update()
		{
			if( Application.isEditor == false ) return;

			if( Input.GetMouseButtonDown(0) )
			{
				HS.Clickable.Selection?.Click();
				if( HS.Clickable.SelectedIsAvatar )
					Debug.Log( $"AVA: {HS.Clickable.SelectedAvatar.gameObject.name}" );
				else if( HS.Clickable.SelectedIsPlatformElement )
				{
					/*
					if( HS.Clickable.SelectedIsPlasmaball )
						Debug.Log( $"PLASM: {HS.Clickable.PlatformObject.name} [{HS.Clickable.ReactMessage}]");
						// unityToReact.SendTeamPlasmaClick( HS.Clickable.ReactMessage );
					else if( HS.Clickable.IsTagged( "@screenLeft" ) ) // case-insensitive
						Debug.Log( $"LEFT: {HS.Clickable.PlatformObject.name} [{HS.Clickable.ReactMessage}]");
						// unityToReact.SendScreen1Click( HS.Clickable.ReactMessage );
					else if( HS.Clickable.IsTagged( "@screenmiddle" ) )  // case-insensitive
						Debug.Log( $"MIDDLE: {HS.Clickable.PlatformObject.name} [{HS.Clickable.ReactMessage}]");
						// unityToReact.SendScreen1Click( HS.Clickable.ReactMessage );
					else if( HS.Clickable.IsTagged( "@screenright" ) )  // case-insensitive
						Debug.Log( $"RIGHT: {HS.Clickable.PlatformObject.name} [{HS.Clickable.ReactMessage}]");
						// unityToReact.SendScreen1Click( HS.Clickable.ReactMessage );
					*/
				}
			}
		}
	}
}