using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;



namespace HS
{
	public class ParticleTransactionCore : MonoBehaviour
	{
		static ParticleTransactionCore _instance;
		
		
		[SerializeField] ParticleTransactor _transactor;
		[SerializeField] List<Color> _colors;


		public static void Add( Vector3 sourcePosition, Vector3 targetPosition, int amount, int type )
		{
			_instance._transactor.Emit(sourcePosition,targetPosition,amount,_instance._colors[type%_instance._colors.Count]);
		}




		void Awake()
		{
			_instance = this;
		}
	}
}