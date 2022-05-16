using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HS
{
    public class GetAWowEvent
    {
        public GetAWowSettings settings;
        public TeamSpaceDriver teamSpaceDriver;
        
		/// <summary> Creates (and destroys) a get-a-wow event. Needs a teamSPaceDriver to get the correct
		/// orientation sorted, HMU if that complicates matters </summary>
        public static void Create(TeamSpaceDriver teamSpaceDriver, GetAWowSettings settings = null)
        {
             DefaultsManager.TriggerDefaulLoad();

            settings = settings ?? GetAWowSettings.Default;
            if (settings.Prefab)
            {
                GameObject prefab = HS.Pool.Instance.GetSpawnFromPrefab(settings.Prefab);
                if( prefab )
                {
                    prefab.transform.position = teamSpaceDriver.transform.position;
                    prefab.transform.rotation = teamSpaceDriver.transform.rotation;
                }
            }
		}

        public static void Create(GameObject gameObject, GetAWowSettings settings = null)
        {
            DefaultsManager.TriggerDefaulLoad();

            settings = settings ?? GetAWowSettings.Default;
            if (settings.Prefab)
            {
                GameObject prefab = HS.Pool.Instance.GetSpawnFromPrefab(settings.Prefab);
                if (prefab)
                {
                    prefab.transform.position = gameObject.transform.position;
                    prefab.transform.rotation = gameObject.transform.rotation;
                }
            }
        }
    }
}
