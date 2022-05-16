using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HS
{
	public class PlasmaballDriver : MonoBehaviour
	{
		public int MeetingRoomID{get;set;}
		public Vector3 Position{get{return transform.position;}set{transform.position = value;}}
		public bool IsFakeTable{get;set;}

		[Header( "Visuals" )]
		[SerializeField] GameObject OpenChatHover;
		[SerializeField] GameObject PrivateChatHover;

		[Header("PlasmaLines")]
		public bool UsePlasmaLines = false;
		[SerializeField] GameObject PlasmaLinePrefab;
		[SerializeField] float PlasmaballLineOffset = 0.12f;
		[SerializeField] float AvatarLineOffset = 0.06f;
		[SerializeField] float PollingDelay = 1; // how many seconds between polling evals (used for disconnection and redrawing the lines)

		[Header( "Line-method 2 params" )]
		[Tooltip( "Only pick the N closest others (in both Angle and Distance) to cast a line to." )]
		[SerializeField] int MaxAvatarConnections = 3;
		[Tooltip( "Avatars need to be at this angle from eachother (relative to the plasmaball) before a connection is considered." )]
		[SerializeField] float MaxAvatarAngle = 45;
		[Tooltip( "Avatars need to be at least this close together before a connection is made." )]
		[SerializeField] float MaxAvatarDistance = 5;

		[Header( "(Visual) Disconnection" )]
		public float BreakoffDistance = 30;

		[Header( "Readout" )]
		public List<AvatarDriver> ConnectedAvatars = new List<AvatarDriver>();

		// working on two methods for creating the 'plasmaball-grid': the lines between the plasmabaal, the avatars and amongst themselves.
		// Method 1:
		// 		Naive approach: simply adds an extra line from the newly-subscribed avatar to the last-subscribed avatar. 
		//		This is cheap and effective, but won't behave well over a network: every person will see a different
		//		pattern.
		// Method 2:
		//		Attempting a deterministic approach: once every N frames (and every time an avatar subscribes/desubscribes) 
		//		we check the distance of all connected avatars, and base the extra network lines on that: every X close-enough avatars
		//		will get an extra connection.
		//		This is more expensive but could lead to better play: every player sees the same network of lines and could conceivably
		//		teamwork a deliberate pattern.
		const int Method = 2;

		Dictionary<PlasmaLine,HashSet<AvatarDriver>> _lines = new Dictionary<PlasmaLine, HashSet<AvatarDriver>>();
		HashSet<PlasmaLine> _extraLines = new HashSet<PlasmaLine>();


		public void SetPrivate( bool isPrivate )
		{
			if( OpenChatHover ) OpenChatHover.SetActive( !isPrivate );
			if( PrivateChatHover ) PrivateChatHover.SetActive( isPrivate );
		}


		/// <summary> For (old) avatar-network display effect, never used </summary>
        public void SubscribeAvatar( AvatarDriver avatar )
		{
			ConnectedAvatars.Add( avatar );

			// Create and Setup primary line to plasmaball
			PlasmaLine lineToBall = HS.Pool.Instance?.GetSpawnFromPrefab( PlasmaLinePrefab )?.GetComponent<PlasmaLine>();
			if( lineToBall == null ) return;
			
			lineToBall.transform.SetParent( transform, true );
			// Debug.Log( $"Adding line-to-ball {lineToBall.name}" );
			_lines.Add( lineToBall, new HashSet<AvatarDriver>{avatar} );
			lineToBall.beginPoint = transform;
			lineToBall.endPoint = avatar.AvatarHover.transform;
			lineToBall.firstOffset = PlasmaballLineOffset;
			lineToBall.secondOffset = AvatarLineOffset;
			lineToBall.UpdatePositions();

			// Create and Setup a line to another avatar
			if (ConnectedAvatars.Count > 1)
			{
				if( Method == 1 )
				{
                    /*
					var secondAvatar = ConnectedAvatars[ConnectedAvatars.Count - 2];

					PlasmaLine lineToOtherAvatar = HS.Pool.Instance?.GetSpawnFromPrefab(PlasmaLinePrefab)?.GetComponent<PlasmaLine>();
					if( lineToOtherAvatar == null ) return;

					_lines.Add( lineToOtherAvatar, new HashSet<AvatarDriver>{avatar,secondAvatar});
					_extraLines.Add( lineToOtherAvatar );
					lineToOtherAvatar.beginPoint = avatar.AvatarHover.transform;
					lineToOtherAvatar.endPoint = secondAvatar.AvatarHover.transform;
					lineToOtherAvatar.firstOffset = AvatarLineOffset;
					lineToOtherAvatar.secondOffset = AvatarLineOffset;
					lineToOtherAvatar.transform.SetParent( transform, true );
					lineToOtherAvatar.UpdatePositions();
                    */
				}
				else if( Method == 2 )
				{
					ConnectClosestAvatars( avatar );

					Dictionary<float, AvatarDriver> avatars = SortNeighboursOf(avatar);

					for (int i = 0; i < avatars.Count; i++)
					{
						if (i == 3)
							break;

						//Getting object of the pool
						PlasmaLine lineToOtherAvatar = HS.Pool.Instance?.GetSpawnFromPrefab(PlasmaLinePrefab)?.GetComponent<PlasmaLine>();
						if (lineToOtherAvatar == null)
							return;

						_lines.Add(lineToOtherAvatar, new HashSet<AvatarDriver> { avatar, avatars.ElementAt(i).Value });
						_extraLines.Add( lineToOtherAvatar );

						AvatarDriver avatarDriver = avatars.ElementAt(i).Value;
						lineToOtherAvatar.beginPoint = avatar.AvatarHover.transform;
						lineToOtherAvatar.endPoint = avatarDriver.AvatarHover.transform;
						lineToOtherAvatar.firstOffset = AvatarLineOffset;
						lineToOtherAvatar.secondOffset = AvatarLineOffset;
						lineToOtherAvatar.transform.SetParent(avatar.transform, true);
						lineToOtherAvatar.UpdatePositions();
					}
				}
			}
		}

		/// <summary> For (old) avatar-network display effect, never used </summary>
		public void DesubscribeAvatar( AvatarDriver avatar )
		{
			// run through the lines that are connected to this avatar and destroy them.
			RemoveAvatarLines( avatar );
			ConnectedAvatars.Remove(avatar);
			HS.Pool.Instance?.Return(avatar.transform.gameObject);
		}



		/// <summary> Draw connecting lines between al avatars.</summary>
		void ConnectAllAvatars()
		{
			foreach( var line in _extraLines )
			{
				if( _lines.ContainsKey( line ) ) _lines.Remove( line );
				HS.Pool.Instance?.Return( line.gameObject );
			}
			_extraLines.Clear();
			var checkList = new HashSet<AvatarDriver>();
			foreach( var avatar in ConnectedAvatars ) ConnectClosestAvatars( avatar, checkList );
		}

		/// <summary> Draws connecting lines between the given avatar and its closest neighbours.
		/// If running throug the hole set, provide a checkList so connections are not drawn twice, please. </summary>
		void ConnectClosestAvatars( AvatarDriver avatar, HashSet<AvatarDriver> checkList = null )
		{
			var closestNeighbours = new Dictionary<float,AvatarDriver>();
			int cnt = 0;
			closestNeighbours = SortNeighboursOf( avatar );
			foreach( var other in closestNeighbours )
			{
				if( other.Value == avatar ) continue;
				cnt++;
				if( cnt > MaxAvatarConnections ) break;
				if( checkList != null && checkList.Contains( other.Value ) ) continue; // dont check closest neighbours that are already evaluated from the other side

				var newLine = HS.Pool.Instance?.GetSpawnFromPrefab( PlasmaLinePrefab )?. GetComponent<PlasmaLine>();
				if( !newLine ) continue;

				_lines.Add( newLine, new HashSet<AvatarDriver> { avatar, other.Value } );
				_extraLines.Add( newLine );

				newLine.beginPoint = avatar.AvatarHover.transform;
				newLine.endPoint = other.Value.AvatarHover.transform;
				newLine.firstOffset = AvatarLineOffset;
				newLine.secondOffset = AvatarLineOffset;
				newLine.transform.SetParent(avatar.transform, true);
				newLine.UpdatePositions();
			}
			if( checkList != null ) checkList.Add( avatar );
		}



		void RemoveAvatarLines( AvatarDriver avatar )
		{
			var removals = new HashSet<PlasmaLine>();
			foreach( var line in _lines.Keys )
				if( _lines[line].Contains( avatar ) )
					removals.Add( line );

			foreach( var line in removals ) 
			{
				_lines.Remove( line );
				HS.Pool.Instance?.Return( line.gameObject );
			}
		}



		void Awake()
		{
			if( PrivateChatHover ) PrivateChatHover.SetActive( false );
		}


		void OnEnable()
		{
			if( UsePlasmaLines ) StartCoroutine( PollingLoop() );
		}
		 
		IEnumerator PollingLoop()
		{
			while( UsePlasmaLines )
			{
				foreach( var avatar in ConnectedAvatars )
				{
					if( (avatar.transform.position-transform.position).sqrMagnitude > BreakoffDistance*BreakoffDistance )
						RemoveAvatarLines( avatar );
				}

				if( Method == 2 ) ConnectAllAvatars();
				yield return new WaitForSeconds( PollingDelay );
			}
		}

		private Dictionary<float, AvatarDriver> SortNeighboursOf(AvatarDriver avatar)
		{
			Dictionary<float, AvatarDriver> avatarDictionary = new Dictionary<float, AvatarDriver>();
			List<float> sqrtMagnitude = new List<float>();
			foreach (AvatarDriver other in ConnectedAvatars)
			{
				if (other == avatar) continue;

				float angleFactor = 
					Mathf.InverseLerp(
						MaxAvatarAngle,
						0,
						Vector3.Angle(
								avatar.transform.position - transform.position,
								other.transform.position - transform.position 
							)
					);
				if( angleFactor <= 0 ) continue;

				float distFactor =
					Mathf.InverseLerp(
						MaxAvatarDistance*MaxAvatarDistance,
						0,
						(other.transform.position - avatar.transform.position).sqrMagnitude
					);
				if( distFactor <= 0 ) continue;

				avatarDictionary.Add( angleFactor * distFactor, other );

				

				// // don't check others if they're on the other side of the plasmaball
				// if(		Vector3.Angle(
				// 			avatar.transform.position - transform.position,
				// 			other.transform.position - transform.position 
				// 		)
				// 		> MaxAvatarAngle
				// )
				// 	continue;

				// float sqrMagnitude = (other.transform.position - avatar.transform.position).sqrMagnitude;

				// if (sqrMagnitude > MaxAvatarDistance)
				// 	continue;

				// avatarDictionary.Add(sqrMagnitude, other);
			}

			avatarDictionary = avatarDictionary.OrderBy( elm => elm.Key).ToDictionary( elm => elm.Key, elm => elm.Value );

			// avatarDictionary = avatarDictionary.OrderBy(obj => obj.Key).ToDictionary(obj => obj.Key, obj => obj.Value);
			// avatarDictionary.Reverse();

			return avatarDictionary;
		}
	}
}