using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class HighFiveTester : MonoBehaviour
    {
		public bool UseRandom = true;
        public AvatarDriver Sender, Reciever;


        [Header( "TESTING" )]
        public bool DoHighFive;
		public KeyCode Key = KeyCode.F2;


		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "High Five";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}


        void Update()
        {
            if( DoHighFive || Input.GetKeyDown( Key ) )
            {
				if( UseRandom )
				{
					(Sender,Reciever) = FindObjectsOfType<AvatarDriver>().Two();
				}
                HighFiveEvent.Create( Sender.transform.position, Reciever.transform.position );
				StartCoroutine( Callout() );
            }
            DoHighFive = false;
        }


    }
}