using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class GetAWowTester : MonoBehaviour
    {
		public bool UseRandom = true;
        public TeamSpaceDriver Team;

        public bool DoGetAWow;
		public KeyCode Key = KeyCode.F3;

		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Get A Wow";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}

        void Update()
        {
            if( DoGetAWow || Input.GetKeyDown( Key ) )
            {
				if( UseRandom ) Team = FindObjectsOfType<TeamSpaceDriver>().One();
                GetAWowEvent.Create( Team );
				StartCoroutine( Callout() );
            }

            DoGetAWow = false;
        }

    }
}
