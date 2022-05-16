using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Nextensions;



namespace HS
{
	public class FakeContentGenerator : MonoBehaviour
	{
		public KeyCode RecalcOnKey = KeyCode.X;
		[FormerlySerializedAs( "ProgramGroups" )]
		public List<ContentGroup> Groups;



		void OnEnable()
		{
			StartCoroutine( SoftStart() );
		}
		
		
		IEnumerator SoftStart()
		{
			yield return null;
			Recalc();
		}

		void Recalc()
		{
			var group = Groups.PickOne();
			var spaces = FindObjectsOfType<UserPlatformDriver>(true);
			foreach( var space in spaces )
			{
				foreach( var id in space.GetPresentPosterTags )
					space.SetPoster( id, group.Posters.PickOne() );
				foreach( var id in space.GetPresentBadgeTags )
					space.SetBadge( id, group.Badges.PickOne() );
				foreach( var id in space.GetPresentScreenTags )
					space.SetScreen( id, group.Screens.PickOne() );
				if( group?.Names.Count > 0 ) space.SetNameRibbon( group.Names.PickOne(), Random.Range(0,group.NamesPerTexture), group.NamesPerTexture );
				space.SetColors( new[]{
					Random.ColorHSV(0,1,0.5f,0.9f,0.5f,0.9f),
					Random.ColorHSV(0,1,0.5f,0.9f,0.5f,0.9f)
				});
			}
		}

		void Update()
		{
			if( Input.GetKeyDown( RecalcOnKey ) ) Recalc();
		}



		[System.Serializable]
		public class ContentGroup
		{
			public List<Texture2D> Screens;
			public List<Texture2D> Posters;
			public List<Texture2D> Badges;
			[Header( "Names" )]
			public int NamesPerTexture = 1;
			public List<Texture2D> Names;
		}
	}

}