using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HS
{
	public static class MadeAPosterEvent
	{
		// private MadeAPosterSettings settings;
		// private TeamSpaceDriver team;
		// private Texture2D poster;
		// public float DestructionTimer = 5;
		
		public static void Create(Vector3 teamLocation, Texture2D poster, MadeAPosterSettings settings = null )
		{
			DefaultsManager.TriggerDefaulLoad();

			settings = settings ?? MadeAPosterSettings.Default;
			var effect = HS.Pool.Instance.GetSpawnFromPrefab( settings.EffectPrefab );
			if( effect )
			{
				effect?.transform.Place( teamLocation );
				var dir = Camera.main.transform.position - effect.transform.position;
				dir.y = 0;
				effect.transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
				effect.GetComponentInChildren<RawImage>( true ).texture = poster;
			}
		}
	}
}