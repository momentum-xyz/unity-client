using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class TeamUpTester : MonoBehaviour
    {
		public bool UseRandom = true;
        public List<TeamSpaceDriver> Teams;

        public bool DoTeamUp;
		public KeyCode Key = KeyCode.F7;


		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Team Up";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}


        void Update()
        {
            if( DoTeamUp || Input.GetKeyDown( Key ) )
            {
				if( UseRandom )
				{
					var (t1,t2) = FindObjectsOfType<TeamSpaceDriver>().Two();
					TeamUpEvent.Create( t1.transform.position, t2.transform.position );
				}
				else
				{
					if( Teams.Count == 2 )
						TeamUpEvent.Create( Teams[0].transform.position,Teams[1].transform.position );
					else if ( Teams.Count > 2 )
					{
						var t1 = Random.Range(0,Teams.Count);
						var t2 = t1;
						while( t2==t1 ) t2 = Random.Range(0,Teams.Count);
						TeamUpEvent.Create( Teams[t1].transform.position, Teams[t2].transform.position );
					}
				}
				StartCoroutine( Callout() );
            }

            DoTeamUp = false;
        }

    }
}
