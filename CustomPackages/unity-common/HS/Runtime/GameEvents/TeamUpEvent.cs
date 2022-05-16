using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    public class TeamUpEvent : MonoBehaviour
    {
        private TeamUpSettings settings;
        private List<TeamSpaceDriver> teams = new List<TeamSpaceDriver>();
        private LineRenderer lineRenderer;
        private GameObject bezier;
        //private int BezierPoints = 50;
        private float height;

        public static void Create( Vector3 teamOneLocation, Vector3 teamTwoLocation, TeamUpSettings settings = null )
        {
            DefaultsManager.TriggerDefaulLoad();
			settings = settings ?? TeamUpSettings.Default;

			// event is built up out of two effects at Team ends, and one connecting the two
			foreach( var op in new[]{teamOneLocation,teamTwoLocation} )
				HS.Pool.Instance.GetSpawnFromPrefab( settings.TeamLocationEffectPrefab )
				?.transform.Place( op );

			HS.Pool.Instance.GetSpawnFromPrefab( settings.ConnectionEffectPrefab )
			?.GetComponentInChildren<ConnectionArcDriver>(true)
			?.Setup( teamOneLocation, teamTwoLocation );
        }
    }
}