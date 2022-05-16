using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class ParticleCollector : MonoBehaviour
	{
		[SerializeField] int _maxParticleCount = 400;

		ParticleSystem _prt;
		ParticleSystem.MainModule _main;
		int _count;


		public void Add( int amount ) => Set( _count + amount );


		public void Set( int count )
		{
			if( !gameObject.activeInHierarchy ) return;
			_count = Mathf.Min( count, _maxParticleCount );
			_main.maxParticles = Mathf.Max(_count,1);
			_prt.Play();
		}


		void Awake()
		{
			_prt = GetComponent<ParticleSystem>();
			_main = _prt.main;
			Set(0);
		}
	}
}