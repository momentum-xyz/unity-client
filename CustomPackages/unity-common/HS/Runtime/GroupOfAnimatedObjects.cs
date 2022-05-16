using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
	public class GroupOfAnimatedObjects : MonoBehaviour
	{
		public GameObject Source;
		public int Amount;
		public float Life = 5;
		public Vector2 MinMaxRange = new Vector2( 2, 5 );
		public float Height = 2;
		public AnimationCurve Envelope = new AnimationCurve( new Keyframe(0,0), new Keyframe(0.5f,1), new Keyframe(1,0) );

		Dictionary<Animator,float> _members = new Dictionary<Animator,float>();

		void Awake()
		{
			Source.SetActive( false );
		}


		void OnEnable()
		{
			StartCoroutine( Spawn() );
			StartCoroutine( Run() );
		}


		IEnumerator Spawn()
		{
			while( _members.Count < Amount )
			{
				var op = Instantiate( Source );
				op.transform.SetParent( transform );
				Vector3 dir = Random.insideUnitCircle.normalized;
				dir.z = dir.y;
				dir.y = 0;
				op.transform.localPosition = dir * Random.Range( MinMaxRange.x, MinMaxRange.y );
				dir.y = Random.Range( -Height/2, Height/2 );
				op.transform.localRotation = Quaternion.AngleAxis( Random.Range(-180,180), Vector3.up );
				op.transform.localScale = Vector3.zero;
				op.SetActive( true );
				_members.Add( op.GetComponent<Animator>(), Life );
				yield return new WaitForSecondsRealtime( Life/Amount );
			}
		}

		IEnumerator Run()
		{
			var removals = new HashSet<Animator>();
			var keys = new List<Animator>();
			while( true )
			{
				removals.Clear();
				yield return new WaitUntil( ()=> _members.Count > 0 );

				while( removals.Count < 1 )
				{
					keys = _members.Keys.ToList();
					foreach( var anm in keys )
					{
						_members[anm] -= Time.unscaledDeltaTime;
						anm.transform.localScale = Vector3.one *Envelope.Evaluate( 1-(_members[anm]/Life) );
						if( _members[anm] <= 0 ) removals.Add( anm );
					}
					yield return null;
				}
				foreach( var op in removals )
				{
					_members.Remove( op );
					Destroy( op.gameObject );
				}
				StopCoroutine( Spawn() );
				StartCoroutine( Spawn() );
			}
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			NHelp.DrawGizmosCircle( transform.position, Vector3.up, MinMaxRange.x );
			NHelp.DrawGizmosCircle( transform.position-Vector3.up*Height/2, Vector3.up, MinMaxRange.y );
			NHelp.DrawGizmosCircle( transform.position+Vector3.up*Height/2, Vector3.up, MinMaxRange.y );
		}
	}
}