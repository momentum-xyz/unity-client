using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
    public class GrabATableTester : MonoBehaviour
    {
		public bool UseRandom = true;
        public List<AvatarDriver> Avatars;
        public GrabATableSettings Settings;


        [Header("TESTING")]
        public bool DoEvent;
		public KeyCode Key = KeyCode.F1;

		public GameObject CalloutPanel;
		IEnumerator Callout()
		{
			if( !CalloutPanel ) yield break;
			CalloutPanel.SetActive( true );
			CalloutPanel.GetComponentInChildren<TMPro.TMP_Text>().text = "Grab A Table";
			yield return new WaitForSeconds( 0.5f );
			CalloutPanel.SetActive( false );
		}



        void Update()
        {

            if( DoEvent || Input.GetKeyDown( Key ) )
            {
				if( UseRandom )
				{
					var avas = FindObjectsOfType<AvatarDriver>();
					if( avas.Length > 1 )
					{
						Avatars.Clear();
						for( int i = 0; i < Random.Range( 0, Mathf.Max( 5, avas.Length ) ); i++ )
						{
							var newAva = avas.One();
							while( Avatars.Contains( newAva ) ) newAva = avas.One();
							Avatars.Add( newAva );
						}
					}
				}
				if( Avatars.Count > 1 )
				{
					Vector3 medPos = Vector3.zero;
					foreach( var ava in Avatars )
						medPos += ava.transform.position;
					medPos /= Avatars.Count;
					GrabATableEvent.CreateWithFireworks( medPos );
					StartCoroutine( Callout() );
				}
            }


            DoEvent = false;
        }
    }
}