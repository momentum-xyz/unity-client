using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class MadeAMemeTester : MonoBehaviour
    {
		public bool UseRandom;
        public TeamSpaceDriver team;
        public Texture2D meme;

        public bool DoMadeAMeme;
		public KeyCode Key = KeyCode.F5;

		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Made A Meme";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}


        void Update()
        {
            if( DoMadeAMeme || Input.GetKeyDown( Key ) )
            {
 				if( UseRandom ) team = FindObjectsOfType<TeamSpaceDriver>().One();
                MadeAMemeEvent.Create(team.transform.position,meme);
				StartCoroutine( Callout() );
            }

            DoMadeAMeme = false;
        }

    }
}
