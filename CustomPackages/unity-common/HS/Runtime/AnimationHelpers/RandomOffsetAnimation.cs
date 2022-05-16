using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;


namespace HS
{
	public class RandomOffsetAnimation : MonoBehaviour
	{
		public RuntimeAnimatorController Controller;
		public Vector2 MinMaxSpeed = new Vector2( 0.9f, 1.2f );
		public string OffsetParameter = "Offset";
		public string SpeedParameter = "Speed";

		Animator _anm;


		void Awake()
		{
			_anm = gameObject.CreateOrAddComponent<Animator>();
			_anm.runtimeAnimatorController = Controller;
			_anm.updateMode = AnimatorUpdateMode.UnscaledTime;
		} 

		void OnEnable()
		{
			StartCoroutine( SoftStart() );
		}


		IEnumerator SoftStart()
		{
			yield return null;
			_anm.SetFloat( OffsetParameter, Random.value );
			_anm.SetFloat( SpeedParameter, Random.Range(MinMaxSpeed.x,MinMaxSpeed.y) );
		}
	}
}
