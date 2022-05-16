using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class CryptoTrekpillDriver : MonoBehaviour
	{
		public Animator Animator;
		public string StateName = "Trek";
		public ParticleSystem Particles;
		public float ParticleDelay = 0.15f;


		public void Trigger()
		{
			StartCoroutine( RunTrigger() );
		}


		void Awake()
		{
			Particles.gameObject.SetActive( false );
		}


		IEnumerator RunTrigger()
		{
			if( Animator.GetCurrentAnimatorStateInfo(0).IsName(StateName) ) yield break;
			Particles.gameObject.SetActive( false );
			Animator.Play( StateName, 0 );
			yield return new WaitForSeconds( ParticleDelay );
			Particles.gameObject.SetActive( true );
			yield return new WaitUntil( ()=> Animator.GetCurrentAnimatorStateInfo(0).IsName(StateName) == false );
			Particles.gameObject.SetActive( false );
		}
	}
}