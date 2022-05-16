using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS.KUSA
{
	public class ScaleWithRelayChainSize : MonoBehaviour
	{
		[SerializeField] float _baseScale = 1;


		void Awake()
		{
			GlobalResponder.OnValidate += Validate;
		}

		void OnDestroy()
		{
			GlobalResponder.OnValidate -= Validate;
		}


		void Validate()
		{
			transform.localScale = Vector3.one * _baseScale *(GlobalResponder.RelayChainRadius/300);
		}
	}
}