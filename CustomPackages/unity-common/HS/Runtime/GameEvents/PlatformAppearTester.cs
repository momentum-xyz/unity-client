using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace HS
{
    public class PlatformAppearTester : MonoBehaviour
    {
        public bool DoAppearPlatform;
		public KeyCode Key = KeyCode.Alpha8;

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
            if( DoAppearPlatform || Input.GetKeyDown( Key ) )
            {
				// make a list of all user spaces
				var ops = new List<Transform>(FindObjectsOfType<TeamSpaceDriver>().Select(p=>p.transform));
				ops.AddRange(FindObjectsOfType<TrackSpaceDriver>().Select(p=>p.transform));
				ops.AddRange(FindObjectsOfType<UserPlatformDriver>().Select(p=>p.transform));

				// pick the closest space at a certain distance in front of the camera
				var cam = Camera.main.transform;
				var pos = cam.position;
				ops.RemoveAll( op => cam.InverseTransformPoint(op.position).z < 10 );
				if( ops.Count <= 0 ) return;
				ops.Sort((a,b)=> (a.transform.position-pos).sqrMagnitude.CompareTo((b.transform.position-pos).sqrMagnitude));

				// experimental: hide object, show it at apex of animation
				var op = ops[0];
				System.Action action = null;
				op.gameObject.SetActive(false);
				action = ()=>op.gameObject.SetActive(true); 

				// LEGACY: check which function we should call
				if( op.GetComponent<UserPlatformDriver>() ) 	PlatformAppearEvent.Create( op.GetComponent<UserPlatformDriver>(), action );
				else if( op.GetComponent<TrackSpaceDriver>() ) 	PlatformAppearEvent.Create( op.GetComponent<TrackSpaceDriver>(), action );
				else if( op.GetComponent<TeamSpaceDriver>() ) 	PlatformAppearEvent.Create( op.GetComponent<TeamSpaceDriver>(), action );
				
				StartCoroutine( Callout() );
            }

            DoAppearPlatform = false;
        }

    }
}
