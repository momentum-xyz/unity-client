using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public class PlasmaTetherStepModule : MonoBehaviour
	{
		// [Range(0,0.5f)] public float ElbowDistance;
		PlasmaTether _tether;
		LineRenderer _line;

		void OnEnable()
		{
			if( !_tether ) _tether = GetComponent<PlasmaTether>();
			if( !_line ) _line = GetComponent<LineRenderer>();
			_tether.OnDraw += Draw;
			_tether.OnLODChange += LODChange;
			// Could also overload OnLODChange
		}


		void OnDisable()
		{
			_tether.OnDraw -= Draw;
			_tether.OnLODChange -= LODChange;
		}


		void LODChange( int newLOD )
		{
			_line.numCornerVertices = newLOD==0 ? 2 : 0;
		}


		Vector3[] _positions;
		void Draw( Vector3 startPoint, Vector3 endPoint )
		{
			var elbow1 = 
				Vector3.Lerp(
					transform.position,
					transform.TransformPoint( endPoint ),
					0.5f
				);

			elbow1.y = transform.position.y;
			var elbow2 = elbow1;
			elbow2.y = transform.TransformPoint(endPoint).y;

			elbow1 = transform.InverseTransformPoint( elbow1 );
			elbow2 = transform.InverseTransformPoint( elbow2 );

			_line.positionCount = 4;
			_line.SetPositions(
				new[]{
					startPoint,
					elbow1,
					elbow2,
					endPoint
				}
			);




			// if( Flat ) transform.rotation = Quaternion.LookRotation( Vector3.down );

			// if( _startShape ) transform.position = _startShape.GetWorldPos( Target.position );
			// else
			// {
			// 	var dir = Target.position - transform.parent.position;
			// 	dir.y = 0;
			// 	transform.position = transform.parent.position + dir.normalized *ParentRadius;
			// }


			// var p0 = Vector3.zero;
			// Vector3 p2;
			// if( _endShape ) p2 = _endShape.GetWorldPos( _startShape ? _startShape.transform.position : transform.parent.position );
			// else
			// {
			// 	var dir = transform.position - Target.position;
			// 	dir.y = 0;
			// 	p2 = Target.position + dir.normalized*TargetRadius;
			// }

			// var p1 = Vector3.Lerp(transform.position, p2, .5f);
			// p1.y = Mathf.LerpUnclamped( p2.y, p0.y, ControlPointYFactor );

			// // Debug.DrawRay( p1, Vector3.up, Color.yellow );
			// // Debug.DrawRay( p2, Vector3.up, Color.black );

			// p1 = transform.InverseTransformPoint( p1 );
			// p2 = transform.InverseTransformPoint( p2 );

			// for (int i = 0; i < BezierPoints; i++)
			// {
			// 	float t = i / ((float)BezierPoints-1);
			// 	_positions[i] = Extensions.CalculateQuadraticBezierPoint(t, p0, p1, p2);
			// 	// Debug.DrawRay( transform.TransformPoint( positions[i] ), Vector3.up, Color.red );
			// }

			// _line.positionCount = BezierPoints;
			// _line.SetPositions(_positions);
		}

	}
}