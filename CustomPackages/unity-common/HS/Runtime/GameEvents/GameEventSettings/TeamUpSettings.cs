using UnityEngine;

namespace HS
{
	[CreateAssetMenu(fileName = "TeamUp Settings", menuName = "ScriptableObjects/TeamUp", order = 0)]
	public class TeamUpSettings : ScriptableObject
	{
		public static TeamUpSettings Default;
 

		public bool IsDefault;
		public GameObject TeamLocationEffectPrefab;
		public GameObject ConnectionEffectPrefab;


		public void Awake()
		{
			OnEnable();
		}

		void OnEnable()
		{
			if (IsDefault)
				Default = this;
		}
	}
}