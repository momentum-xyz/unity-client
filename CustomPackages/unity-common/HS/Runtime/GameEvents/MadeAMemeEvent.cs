using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HS
{
    public static class MadeAMemeEvent
    {
        // private MadeAMemeSettings settings;
        // private TeamSpaceDriver team;
        // private Texture2D meme;
        // public float DestructionTimer = 5;


        public static void Create(Vector3 teamLocation, Texture2D meme,MadeAMemeSettings settings = null)
        {
  			DefaultsManager.TriggerDefaulLoad();

 			settings = settings ?? MadeAMemeSettings.Default;
			var effect = HS.Pool.Instance.GetSpawnFromPrefab( settings.EffectsPrefab );
			if( effect )
			{
				effect.transform.position = teamLocation;
				var dir = Camera.main.transform.position - effect.transform.position;
				dir.y = 0;
				effect.transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
				effect.GetComponentInChildren<RawImage>( true ).texture = meme;
			}
        }

    }
}