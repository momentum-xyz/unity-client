using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class ExhaustDriver : MonoBehaviour
	{
		[SerializeField] string _boostName = "Boost";


		Animator _anm;


		public void SetBoost( float boost )
		{
			_anm.SetFloat( _boostName, boost );
		}


		void Awake()
		{
			_anm = GetComponent<Animator>();
		}

	}
}