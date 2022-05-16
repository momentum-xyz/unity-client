using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
	/// <summary> Draws a linerenderer rc between two locations (or transforms) 
	/// in space. </summary>
	public class ConnectionArcDriver : MonoBehaviour
	{
		public Transform Location1,Location2;
		public Vector3 Position1,Position2;
		public bool OnlyUpdateOnce = true;
		public int LineResolution = 20;
		public float HeightFactor = 0.35f;

		LineRenderer _line;
		float _height;
		bool _hasDrawn;


		public void Setup( Vector3 loc1, Vector3 loc2 )
		{
			Location1 = null;
			Location2 = null;
			Position1 = loc1;
			Position2 = loc2;
			OnlyUpdateOnce = true;
			_hasDrawn = false;
		}
		public void Setup( Transform loc1, Transform loc2 )
		{
			Location1 = loc1;
			Location2 = loc2;
			OnlyUpdateOnce = false;
			_hasDrawn = false;
		}




        void OnEnable()
        {
			_hasDrawn = false;
		}


        void Update()
		{
            if( !OnlyUpdateOnce || !_hasDrawn  ) DrawArc();
		}


		[ContextMenu( "Draw" )]
        void DrawArc()
        {
			if( !_line )
			{
				_line = GetComponentInChildren<LineRenderer>();
				_line.useWorldSpace = true;
				_line.alignment = LineAlignment.TransformZ;
			}
			
			Vector3 p0,p1,p2;
			if( Location1 && Location2 )
			{
				p0 = Location1.position;
				p2 = Location2.position;
			}
			else
			{
				p0 = Position1;
				p2 = Position2;
			}
			p1 = (p0+p2)/2f + Vector3.up *(p2-p0).magnitude *HeightFactor;

			_line.transform.position = p1;
			_line.transform.rotation = Quaternion.LookRotation( Vector3.up );

        	var positions = new Vector3[LineResolution];

            for (int i = 0; i < LineResolution; i++)
                positions[i] = 
					Extensions.CalculateQuadraticBezierPoint( 
						i / ((float)LineResolution - 1), 
						p0, 
						p1, 
						p2 
					);

            _line.positionCount = LineResolution;
            _line.SetPositions(positions);
        }
	}
}
