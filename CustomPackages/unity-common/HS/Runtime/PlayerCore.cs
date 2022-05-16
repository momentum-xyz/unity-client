using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




namespace HS
{
	public class PlayerCore : MonoBehaviour
	{
		static PlayerCore _instance;
		static AvaRole _role = AvaRole.TeamMember;

		[SerializeField] List<AvaCoreDescription> _definitions;
		[SerializeField] AvaRole SetInEditor = AvaRole.TeamMember;

		[System.Serializable] class AvaCoreDescription{
			public AvaRole Role;
			public GameObject Prefab;
			public AvaCoreDescription( AvaRole role, GameObject prefab )
			{
				Role = role;
				Prefab = prefab;
			}
		}

		AvatarDriver _driver;

		public static void SetPlayerCore( AvaRole role )
		{
			_role = role;
			_instance?.ForceSetRole( _role );
		}


		void Awake()
		{
			_driver = GetComponent<AvatarDriver>();
			_instance = this;
		}

		void OnDestroy()
		{
			if( _instance == this ) _instance = null;
		}


		void ForceSetRole( AvaRole newRole )
		{
			SetInEditor = newRole;
			var newSource = _definitions.FirstOrDefault( elm=> elm.Role == newRole );
			var curCore = _driver.AvatarHover?.gameObject;
			if( curCore != null && newSource != null )
			{
				var op = Instantiate( newSource.Prefab );
				op.transform.SetParent( curCore.transform.parent );
				op.transform.Zero();
				op.SetActive( true );
				_driver.AvatarHover = op.GetComponent<WanderHover>();
				Destroy( curCore.gameObject );
				SetInEditor = newRole;
			}
		}


        /*
                // is this needed?
		void Update()
		{            
			if( _role != SetInEditor )
			{
				_role = SetInEditor;
				SetPlayerCore( SetInEditor );
			}
            
		}
        */
	}
}