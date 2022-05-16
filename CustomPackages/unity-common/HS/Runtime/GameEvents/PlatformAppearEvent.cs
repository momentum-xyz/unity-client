using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nextensions;

namespace HS
{
    public static class PlatformAppearEvent
    {
		public static void Create( UserPlatformDriver platform, System.Action apexAction, PlatformAppearSettings settings = null )
			=> Create( platform.transform, platform.TetherRingRadius, apexAction, settings );
		public static void Create( TrackSpaceDriver platform, System.Action apexAction, PlatformAppearSettings settings = null )
			=> Create( platform.transform, platform.TetherRingRadius, apexAction, settings );
		public static void Create( TeamSpaceDriver platform, System.Action apexAction, PlatformAppearSettings settings = null )
			=> Create( platform.transform, platform.TetherRingRadius, apexAction, settings );



        public static void Create( Transform platform, float radius, System.Action apexAction, PlatformAppearSettings settings = null )
        {
  			DefaultsManager.TriggerDefaulLoad();

 			settings = settings ?? PlatformAppearSettings.Default;
			var effect = HS.Pool.Instance.GetSpawnFromPrefab( settings.EffectsPrefab );
			var actor = effect.GetComponentInChildren<AnimationAction>();
			if( actor ) actor.OnAction += apexAction;
			if( effect )
			{
				var p = effect.transform.parent;
				effect.transform.SetParent( platform, false );
				effect.transform.Zero();
				effect.transform.localScale = Vector3.one * (radius/effect.GetComponent<ReferenceRadius>().Radius);
				effect.transform.SetParent( p, true );
			}
        }
    }
}