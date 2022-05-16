using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;
using TMPro;

namespace HS
{
	public class RadialCloner : MonoBehaviour
	{
		public GameObject Original;
		public int Count = 12;
		public float Radius = 1;

		public float RandomRadius = 0;

		public Vector3 PointTowardsCenter = Vector3.zero;

		public bool StickToGizmo;

		[Space]
		public MultiSurfaceDriver AnnounceSurfs;
		public bool DistributeCompassLabels;

		public bool IsSetup{get;private set;}


		void Start()
		{
			var oPos = Vector3.right * Radius;
			var axis = Vector3.up;
			Original.transform.position = transform.TransformPoint(oPos);
			for (int i = 0; i < Count; i++)
			{
				Transform newOp = null;
				if( i > 0 )
				{
					oPos = Quaternion.AngleAxis(360f / Count, axis) * oPos;
					newOp = Instantiate(Original).transform;
					newOp.SetParent(Original.transform.parent, true);
					newOp.position = transform.TransformPoint(oPos);
					newOp.localScale = Original.transform.localScale;
					if (RandomRadius > 0)
					{
						newOp.position += Random.insideUnitSphere * RandomRadius;
					}
				}
				else
					newOp = Original.transform;

				if( PointTowardsCenter != Vector3.zero )
				{
					newOp.rotation = 
						Quaternion.FromToRotation(
							newOp.TransformDirection( PointTowardsCenter )
							,
							transform.TransformDirection(
								Vector3.Scale(
									-newOp.localPosition,
									new Vector3(1,0,1)
								)
							)
						)
						*newOp.rotation;
						;
				}
			}
			AnnounceSurfs?.Setup(transform);
			if( DistributeCompassLabels ) SetupCompassLabels();
			IsSetup = true;
		}



		void SetupCompassLabels()
		{

			var compassDirs = 
					transform.FindByTaggedNameAll<TMP_Text> ( "@compass" );
					// GetComponentsInChildren<TMP_Text>( true )
					// .Where<TMP_Text>( elm=>elm.name.ToUpper().Contains( "[COMPASS]" ))
					// ;
			foreach( var elm in compassDirs )
			{
				var angle = Vector3.SignedAngle( 
								Vector3.forward, 
								Vector3.Scale( transform.InverseTransformPoint(elm.transform.position), new Vector3(1,0,1) ), 
								Vector3.up 
							);
				angle = (angle+360)%360;
				var idx = ((int)(angle/360f*16f+0.5f))%16;
				elm.text = 
					"N,nne,NE,nee,E,see,SE,sse,S,ssw,SW,sww,W,nww,NW,nnw"
					.Split(',')
					[idx];
			}
		}



		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			NHelp.DrawGizmosCircle(transform, Vector3.up, Radius);

			if (RandomRadius > 0)
				Gizmos.DrawWireSphere(transform.position + transform.right * Radius, RandomRadius);

			if( StickToGizmo && Application.isPlaying == false && Original != null )
				Original.transform.localPosition = Vector3.forward * Radius;
		}




	}
}