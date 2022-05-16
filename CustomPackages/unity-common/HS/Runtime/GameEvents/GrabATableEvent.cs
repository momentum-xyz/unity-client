using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	/// <summary> A GrabATable Event happens when two avatars decide to open a private chat. A plasmaball will spawn,
	/// and the avatars will (optionally) gather around it. An effect spawns to attract attention to it.
	/// The effect is meant to be welcoming for others to join, so the plasmaball should be accessible to other players.
	/// </summary>
	public static class GrabATableEvent
	{
		/// <summary> Generates just the fireworks that announce a NEW table opening up </summary>
		public static void CreateFireworks( Vector3 worldLocation )
		{
			DefaultsManager.TriggerDefaulLoad();
			var settings = GrabATableSettings.Default;

			var effect = HS.Pool.Instance.GetSpawnFromPrefab( settings.AppearEffectPrefab );
			if( effect != null ) effect.transform.position = worldLocation;

		}
		/// <summary> Generates a GrabATable event (including fireworks) at the given location </summary>
		public static PlasmaballDriver CreateWithFireworks( Vector3 worldLocation )
		{
			CreateFireworks( worldLocation );
			return CreateTable( worldLocation );
		}
		/// <summary> Spawns in a Plasmaball/table combo at the desired location, no fireworks
		/// (meant for visualizing already-present tables ) </summary>
		public static PlasmaballDriver CreateTable( Vector3 worldLocation )
		{
			DefaultsManager.TriggerDefaulLoad();
			var settings = GrabATableSettings.Default;

			var plasmaBall = Pool.Instance.GetSpawnFromPrefab( settings.PlasmaballPrefab )?.GetComponent<PlasmaballDriver>();
			if( plasmaBall != null )
				plasmaBall.transform.position = worldLocation;
			return plasmaBall;
		}
	}
}