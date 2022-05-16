using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class MadeAPosterTester : MonoBehaviour
    {
		public bool UseRandom;
        public TeamSpaceDriver Team;
        public Texture2D Poster;

        public bool DoMadeAPoster;
		public KeyCode Key = KeyCode.F4;

		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Made A Poster";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}


        void Update()
        {
            if( DoMadeAPoster || Input.GetKeyDown( Key ) )
            {
				if( UseRandom ) Team = FindObjectsOfType<TeamSpaceDriver>().One();
                MadeAPosterEvent.Create(Team.transform.position,Poster);
				StartCoroutine( Callout() );
            }

            DoMadeAPoster = false;
        }

    }
}
