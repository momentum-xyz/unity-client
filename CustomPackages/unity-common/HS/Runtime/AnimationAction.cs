using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class AnimationAction : MonoBehaviour
	{
		public System.Action OnAction;

		void OnCallAction()
		{
			OnAction?.Invoke();
		}
	}
}
