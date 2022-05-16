using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
	[CreateAssetMenu()]
	public class Awards : ScriptableObject
	{
		[System.Serializable]
		public class AwardDefinition
		{
			public string Team;
			public Texture2D RGBA;
			public Texture2D Normal;
		}

		public List<AwardDefinition> List;


		public AwardDefinition Find( string team ) =>
			List.FirstOrDefault<AwardDefinition>( (elm)=>elm.Team==team );

	}
}