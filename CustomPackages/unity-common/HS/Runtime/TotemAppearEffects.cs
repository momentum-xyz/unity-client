using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class TotemAppearEffects : MonoBehaviour
	{
		public GameObject AppearEffectPrefab;
		public ArrangeSpacesInteractively Arranger;
		public Transform ProbeLocation;
		public float SpawnDistance = 100;
		public float Scale = 5;


		IEnumerator Start()
		{
			yield return new WaitUntil( ()=> Arranger.DidArrange );

			var locations = Arranger.GetTeamTotemLocations();

			while( locations.Count > 0 )
			{
				Transform winner = null;
				foreach( var elm in locations )
				{
					if( 
							ProbeLocation.position.y > elm.position.y						// only when above the plane of totems
						&& 	ProbeLocation.InverseTransformPoint( elm.position ).z > 60		// only stuff ahead
						&& 	(elm.position-ProbeLocation.position).sqrMagnitude<SpawnDistance*SpawnDistance 
					  )
					{
						winner = elm;
						var op = HS.Pool.Instance.GetSpawnFromPrefab( AppearEffectPrefab );
						op.transform.Place( winner );
						op.transform.localScale = Vector3.one*Scale;
						break;
					}
				}
				locations.Remove( winner );
				yield return null;
			}
		}
	}
}