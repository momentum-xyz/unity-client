using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS.KUSA
{
	public static class GlobalResponder
	{
		public static float RelayChainRadius{get=>_relayChainRadius;set{_relayChainRadius = value; Validate();}}
		static float _relayChainRadius = 300;


		public static System.Action OnValidate;


		static void Validate()
		{
			OnValidate?.Invoke();
		}
	}
}