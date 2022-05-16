using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HS
{
	public class JoinThePlanEvent : MonoBehaviour
	{
		public JoinThePlanSettings Settings;
		public Vector3 TeamLocation;
		public Vector3 AvatarLocation;

		
		// TODO: Add parameters to this function
		public static JoinThePlanEvent Create(Vector3 teamLocation, Vector3 avatarLocation, JoinThePlanSettings settings = null)
		{
			DefaultsManager.TriggerDefaulLoad();

			JoinThePlanEvent result = new GameObject("JoinThePlanHandler").AddComponent<JoinThePlanEvent>();
			result.Settings = (settings != null ) ? settings : JoinThePlanSettings.Default;
			result.TeamLocation = teamLocation;
			result.AvatarLocation = avatarLocation;
			return result;
		}

		IEnumerator Start()
		{
			HS.Pool.Instance.GetSpawnFromPrefab( Settings.AvatarEffect )
			?.transform.Place( AvatarLocation );

			yield return new WaitForSeconds( 0.5f );

			HS.Pool.Instance.GetSpawnFromPrefab( Settings.TeamSpaceEffect )
			?.transform.Place( TeamLocation );

			HS.Pool.Instance.GetSpawnFromPrefab( Settings.ConnectionEffect )
			?.GetComponentInChildren<ConnectionArcDriver>(true)
			?.Setup( AvatarLocation, TeamLocation );

			Destroy( gameObject );
		}
	}
	
}
