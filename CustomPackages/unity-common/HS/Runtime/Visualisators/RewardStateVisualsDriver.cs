using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;



namespace HS
{
	public class RewardStateVisualsDriver : MonoBehaviour
	{
		[SerializeField] ParticleSystem _rewardenLights;
		[SerializeField] ParticleSystem _shower;
		[SerializeField] MeshFilter _locationMesh;
		[SerializeField] Vector2 _minMaxRewardAmount = new Vector2( 1, 2000 );
		[SerializeField] int _showerAmount = 3000;

		[Header( "PROTO" )]
		// [SerializeField] float _rewardsPerSecond = 100;
		// [SerializeField] float _mannaThreshold = 200;

		ParticleSystem.MainModule _main;
		ParticleSystem.EmissionModule _emit;

		float _accumReward = 0;

		Vector3[] _p;



		/// <summary> Set the current accumulated amount of reward, normalized to the maximum. </summary>
		public void SetRewards( float normalizedRewardsState )
		{
			_accumReward = Mathf.Lerp( _minMaxRewardAmount.x, _minMaxRewardAmount.y, normalizedRewardsState );
			_rewardenLights.Play();
		}


		/// <summary> Signal that rewards are being handed out, so that the shower visuals can be kicked in. </summary>
		public void Shower()
		{
			_accumReward = 0;
			_rewardenLights.Stop();
			_rewardenLights.Clear();
			_shower.Play();
		}


		void Awake()
		{
			_main = _rewardenLights.main;
			_emit = _rewardenLights.emission;
			_main.maxParticles = 0;
			if( _locationMesh )
				_p = _locationMesh.sharedMesh.vertices;
			_shower.Stop();
		}



		void Update()
		{
			_main.maxParticles = (int)_accumReward;
			if( _locationMesh )
				_rewardenLights.transform.position = _locationMesh.transform.TransformPoint(_p.PickOne());
			
			_rewardenLights.Emit( Mathf.Max( (int)_accumReward-_rewardenLights.particleCount, 5 ) );
		}

	}
}