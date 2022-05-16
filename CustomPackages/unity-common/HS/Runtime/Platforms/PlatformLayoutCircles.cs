using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
	public class PlatformLayoutCircles : MonoBehaviour, IPlatformLayouter 
	{
		public Circler ProgramSpaces;
		public Circler ChallengeSpaces;
		public Circler TeamSpaces;


		HashSet<GameObject> _collection = new HashSet<GameObject>();


		public void Generate( GameObject programSpacePrefab, GameObject challengeSpacePrefab, GameObject teamSpacePrefab )
		{
			if( !programSpacePrefab || !challengeSpacePrefab || !teamSpacePrefab ) return;

			foreach( var op in _collection ) Destroy( op );
			_collection.Clear();

			foreach( var ps in ProgramSpaces.Generate( programSpacePrefab, null ) )
			{
				_collection.Add( ps.gameObject );
				foreach( var cs in ChallengeSpaces.Generate( challengeSpacePrefab, ps ) )
				{
					_collection.Add( cs.gameObject );
					foreach( var ts in TeamSpaces.Generate( teamSpacePrefab, cs ) )
					{
						_collection.Add( ts.gameObject );
					}
				}
			}
		}


		public void DrawGizmos()
		{
			Gizmos.color = Color.blue;
			NHelp.DrawGizmosCircle( Vector3.zero, Vector3.up, ProgramSpaces.Radius );
			Gizmos.color = Color.cyan;
			NHelp.DrawGizmosCircle( 
				Vector3.right*ProgramSpaces.Radius + Vector3.up*ChallengeSpaces.MinMaxHeight.x, 
				Vector3.up, 
				ChallengeSpaces.Radius 
			);
			NHelp.DrawGizmosCircle( 
				Vector3.right*ProgramSpaces.Radius + Vector3.up*ChallengeSpaces.MinMaxHeight.y, 
				Vector3.up, 
				ChallengeSpaces.Radius 
			);
			Gizmos.color = Color.white;
			NHelp.DrawGizmosCircle( 
				Vector3.right*(ProgramSpaces.Radius+ChallengeSpaces.Radius) 
					+ Vector3.up*(ChallengeSpaces.MinMaxHeight.x+TeamSpaces.MinMaxHeight.x), 
				Vector3.up, 
				TeamSpaces.Radius 
			);
			NHelp.DrawGizmosCircle( 
				Vector3.right*(ProgramSpaces.Radius+ChallengeSpaces.Radius) 
					+ Vector3.up*(ChallengeSpaces.MinMaxHeight.y+TeamSpaces.MinMaxHeight.y), 
				Vector3.up, 
				TeamSpaces.Radius 
			);


		}


		[System.Serializable]
		public class Circler
		{
			public float Radius = 50;
			public Vector2 MinMaxCount = new Vector2( 2,6 );
			public Vector2 MinMaxHeight = new Vector2( 10, 30 );
			public bool FaceParent = true;


			public HashSet<Transform> Generate( GameObject prefab, Transform parent = null )
			{
				var cnt = (int)Random.Range( MinMaxCount.x, MinMaxCount.y );
				var result = new HashSet<Transform>();
				for( int i = 0; i < cnt; i++ )
				{
					var op = Instantiate( prefab ).transform;
					if( parent ) op.SetParent( parent, true );
					op.position = 
						(parent
							?parent.transform.position
							:Vector3.zero )
						+ Quaternion.AngleAxis( (float)i/cnt *360f, Vector3.up )
							*Vector3.right
							*Radius
						+ Vector3.up * Random.Range( MinMaxHeight.x, MinMaxHeight.y )
						;
					if( FaceParent )
						op.rotation = 
							Quaternion.LookRotation(
								Vector3.Scale( 
									new Vector3(1,0,1),
									(parent
										? parent.position
										: Vector3.zero)
									-op.position
								)
								,
								Vector3.up
							);
					result.Add(op);
				}
				return result;
			}
		}
	}
}