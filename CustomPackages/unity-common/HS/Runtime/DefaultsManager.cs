using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class DefaultsManager
	{
		static bool _loaded;
		public static void TriggerDefaulLoad()
		{
			if( _loaded ) return;
			Resources.LoadAll( "SettingsAndDefaults" );
			_loaded = true;
		}
	}
}