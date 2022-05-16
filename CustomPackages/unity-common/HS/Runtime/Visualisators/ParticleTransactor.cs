using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;

namespace HS
{
	public class ParticleTransactor : MonoBehaviour
	{
		#region STATIC

		static List<ParticleTransactor> _members = new List<ParticleTransactor>();
		public static ParticleTransactor RandomTarget() => _members.PickOne();

		#endregion


		public int Count => _generator!=null?_generator.particleCount:0;


		[SerializeField] ParticleSystem _generator;
		[SerializeField] bool _targetByChannel;
		[SerializeField] HS.ParticleCollector _collector;
		[SerializeField] float _collectRadius = 20;


		[Header( "Motion" )]
		[SerializeField] float _orbitForce = 20;
		[SerializeField] [Range(0,1)] float _absolutionFactor = 0.25f; // what timerange towards the end of lifetime is dedicated to absolutely reaching the goal
		[SerializeField] float _absolutionCurve = 60; // PowerCurve shifting that range towards the absolute end.
		[SerializeField] float _damping = 0;


		Transform _cam;
		ParticleSystem.MainModule _main;
		ParticleSystem.EmissionModule _emit;

		ParticleSystem.Particle[] _generatorParticles;
		List<Vector4> _customParticleStream = new List<Vector4>();



		public void Emit( Vector3 sourcePos, int count, Color color ) => Emit( sourcePos, transform.position, count, color );
		public void Emit( Vector3 sourcePos, Vector3 targetPos, int count, Color color )
		{
			if( !_cam ) _cam = Camera.main?.transform;
			if( _cam ) _generator.transform.LookAt( _cam );

			_generator.transform.position = sourcePos;
			var m = _generator.main;
			m.startColor = color;

			_generator.Emit( count );

			if( _targetByChannel ) // drop the given target position into a custom particle data slot
			{
				// if we set the CustomData in ParticleSystem.CustomDataModule before emit (ie: have it enabled on the ParticleSystem
				// inside the editor), it doesn't "stick" with the particle: changing it at a later time will shange it on -all- alive 
				// particles. In that way, it works differently from aspects like color and size (which 'stick' with the particle after spawn).
				// (would love to be proven wrong here, but I tried in all sorts of way, Maybe I'm not understanding the MinMaxCurve setup
				// of the particle modules).
				// The only way to make CustomData values 'stick' is to disable the module, and manually apply it to the particle 
				// after spawn. So below, we run through them all, decide which ones must have just been spawned a few lines ago, then set the
				// custom target on them.
				// (naam)
				var cnt = _generator.GetParticles(_generatorParticles);
				var customData = new List<Vector4>();
				_generator.GetCustomParticleData( customData, ParticleSystemCustomData.Custom1 );
				if( targetPos.sqrMagnitude <= 0.00001f ) targetPos = new Vector3(0.01f,0,0); // make sure particles never target absolute zero
				for( int i = 0; i < cnt; i++ ) 
					// if( _generatorParticles[i].remainingLifetime == _generatorParticles[i].startLifetime ) // lifetime will be true of -all- particles spawned this frame, o a bad choice to check
					if( customData[i].sqrMagnitude <= 0.00001f ) // should only be true if the particle has -just- been spawned
						customData[i]=(Vector4)targetPos;
				_generator.SetCustomParticleData( customData, ParticleSystemCustomData.Custom1 );
			}
		}



		void Awake()
		{
			_main = _generator.main;
			_emit = _generator.emission;
			_main.playOnAwake = false;
			_generator.Stop();
			_generatorParticles = new ParticleSystem.Particle[_generator.main.maxParticles];
		}


		void OnEnable() => _members.Add(this);
		void OnDisable() => _members.Remove(this);




		void Update()
		{
			UpdateAllParticles();
		}



		void UpdateAllParticles()
		{
			var cnt = _generator.GetParticles( _generatorParticles );
			if( cnt <= 0 ) return;
			if( _targetByChannel ) _generator.GetCustomParticleData( _customParticleStream, ParticleSystemCustomData.Custom1 );
			for( int i = 0; i < cnt; i++ )
			{
				var p = _generatorParticles[i];
				if( p.remainingLifetime <= 0 ) continue;
				// var phase =  1-(p.remainingLifetime/p.startLifetime);
				var targetPos = 
					_targetByChannel
						? (Vector3)_customParticleStream[i]
						: transform.position;//Vector3.Lerp( p.position+p.velocity*Time.deltaTime, Attractor.position, phase );
				// Debug.Log( $"PRT[{i}]: {targetPos}" );
				var phase = p.startLifetime>0?(p.startLifetime-p.remainingLifetime)/p.startLifetime:0;
				if( phase >= 0.999f || (p.position-targetPos).sqrMagnitude <= _collectRadius*_collectRadius )
				{
					// stuff that happens on kill
					// Attractor.Add(1);
					if( _collector ) _collector.Add(1); // TODO: don't do this per source particle, but collect them
					// p.startColor = Color.red;
					// p.startLifetime = 0;
				}
				else
				{
					var targetVelo = p.remainingLifetime>0?((targetPos-p.position)/p.remainingLifetime):Vector3.zero;

					//  basic steering behaviour
					var r = Quaternion.FromToRotation(p.velocity,targetVelo);
					var nVelo = 
						Quaternion.Lerp(
							Quaternion.identity,
							r,
							Time.deltaTime*phase*phase*_orbitForce
						)
						*p.velocity
					;
					nVelo = (1-(Time.deltaTime*_damping))*nVelo;
					p.velocity = Vector3.Lerp(nVelo, targetVelo, Mathf.Pow(Mathf.InverseLerp(1-_absolutionFactor,1,phase),_absolutionCurve) );
				}

				_generatorParticles[i]=p;
			}
			_generator.SetParticles( _generatorParticles, cnt );
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere( transform.position, _collectRadius );
		}
	}
}
