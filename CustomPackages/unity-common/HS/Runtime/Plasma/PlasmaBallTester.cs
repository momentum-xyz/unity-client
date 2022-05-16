using UnityEngine;

namespace HS
{
	public class PlasmaBallTester : MonoBehaviour
	{
		[SerializeField] KeyCode AddAvatarButton = KeyCode.PageUp;
		[SerializeField] KeyCode RemoveAvatarButton = KeyCode.PageDown;
		[SerializeField]
		private Pool pool;
		[SerializeField]
		private GameObject avatarPrefab;
		[SerializeField]
		private PlasmaballDriver plasmaBall;

		public Vector2 MinMaxSpawnDistance = new Vector2( 0.5f, 4f );
		public float SpawnHeight = 1;


		private void Update()
		{
			if (Input.GetKeyUp(AddAvatarButton))
				GenerateAvatar();
			if (Input.GetKeyUp(RemoveAvatarButton) && plasmaBall.ConnectedAvatars.Count>0)
				plasmaBall.DesubscribeAvatar(plasmaBall.ConnectedAvatars[plasmaBall.ConnectedAvatars.Count - 1]);
		}

		private void GenerateAvatar()
		{
			var newAvatar = pool.GetSpawnFromPrefab(avatarPrefab)?.GetComponent<AvatarDriver>();
			if (newAvatar == null)	return;

			var newPos = 
				plasmaBall.transform.position
				+ Vector3.Scale(
					Random.onUnitSphere 
						* Random.Range( MinMaxSpawnDistance.x, MinMaxSpawnDistance.y ),
					new Vector3( 1, SpawnHeight/MinMaxSpawnDistance.y, 1 )
				);
			newAvatar.transform.position = newPos;
			plasmaBall.SubscribeAvatar(newAvatar);
		}
	}
}