using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace HS 
{
	[CreateAssetMenu(fileName = "Avatar Role Settings", menuName = "ScriptableObjects/Avatar Role Settings", order = 1)]
	public class AvatarRolesAndColors : ScriptableObject
	{
		public static AvatarRolesAndColors Default;

		[FormerlySerializedAs( "Settings" )]
		public List<RoleColor> RoleDefinitions;

		Dictionary<AvaRole,RoleColor> _roles = new Dictionary<AvaRole,RoleColor>();

		/// <summary> Returns the default color associated with given avatar role </summary>
		public static Color GetColor( AvaRole role )
		{
			DefaultsManager.TriggerDefaulLoad();
			return Default._roles[role].Color;
		}  
		/// <summary> Returns the default Prefab associated with given avatar role </summary>
		public static GameObject GetLOD0Prefab( AvaRole role )
		{
			DefaultsManager.TriggerDefaulLoad();
			return Default._roles[role].Prefab;
		}


		void Awake()
		{
			Default = this;
			_roles.Clear();
			foreach( var def in RoleDefinitions )
				_roles.Add( def.Role, def );
		}

		[System.Serializable]
		public struct RoleColor
		{
			public AvaRole Role;
			public Color Color;
			[Tooltip( "For LOD0")]
			/// <summary> For LOD0 </summary>
			public GameObject Prefab;
		}

	}
}