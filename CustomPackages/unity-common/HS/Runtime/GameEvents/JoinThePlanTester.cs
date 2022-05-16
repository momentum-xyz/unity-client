using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS
{
    public class JoinThePlanTester : MonoBehaviour
    {
		public bool UseRandom;
        public TeamSpaceDriver team;
        public AvatarDriver avatar;

        public bool DoJoinThePlan;
		public KeyCode Key = KeyCode.F6;

		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Joint The Plan";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}


        void Update()
        {
            if( DoJoinThePlan || Input.GetKeyDown( Key ) )
            {
				if( UseRandom )
				{
					team = FindObjectsOfType<TeamSpaceDriver>().One();
					avatar = FindObjectsOfType<AvatarDriver>().One();
					// var teams = FindObjectsOfType<TeamSpaceDriver>();
					// var avatars = FindObjectsOfType<AvatarDriver>();
					// team = teams[Random.Range(0,teams.Length)];
					// avatar = avatars[Random.Range(0,avatars.Length)];
				}
                JoinThePlanEvent.Create(team.transform.position,avatar.transform.position);
				StartCoroutine( Callout() );
            }
            DoJoinThePlan = false;
        }



    }
}