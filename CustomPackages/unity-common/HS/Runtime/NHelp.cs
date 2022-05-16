using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


/// <summary>
/// NHelp -- a collection of static helper functions
/// </summary>
public static class NHelp 
{
	public static bool CanInvoke<T>( this System.Action<T> action ) =>
		action != null && action.GetInvocationList().Length > 0;


	/// <summary>
	/// Scales the target relatively around an arbitrary point by scaleFactor.
	/// The pivot is assumed to be the position in the space of the target.
	/// Scaling is applied to localScale of target.
	/// </summary>
	public static void ScaleAround( this Transform target, Vector3 pivot, float scaleFactor)
	{
		// pivot
		var pivotDelta = target.localPosition - pivot;
		pivotDelta.Scale( Vector3.one * scaleFactor);
		target.localPosition = pivot + pivotDelta;
	
		// scale
		var finalScale = target.localScale;
		finalScale.Scale( Vector3.one * scaleFactor );
		target.localScale = finalScale;
	}
	
	
	public static Transform FindByName(Transform op, string name)
	{
		if (op.name==name) return op;
		Transform result=null;
		foreach(Transform child in op)
		{
			result=FindByName (child,name);
			if (result) break;
		}
		return result;
	}

	/// <summary> returns the build index of a named scene, even if it isn't loaded.
	/// Returns -1 if non-existent. </summary>
	public static int BuildIndexFromName(string sceneName) 
	{
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) 
		{
			string testedScreen = BuildNameFromIndex(i);
			if (testedScreen == sceneName)
				return i;
		}
		return -1;
	}
	/// <summary> Returns the name of a scene by build index. The scene doesn't have 
	/// to be loaded for this. </summary>
	public static string BuildNameFromIndex( int BuildIndex )
	{
		string path = SceneUtility.GetScenePathByBuildIndex( BuildIndex );
		int slash = path.LastIndexOf('/');
		string name = path.Substring(slash + 1);
		int dot = name.LastIndexOf('.');
		return name.Substring(0, dot);
	}

	public static Vector3 PerlinVector3(float time)
	{
		float x = Mathf.PerlinNoise (time, 0)-0.5f;
		float y = Mathf.PerlinNoise (time+100, 100)-0.5f;
		float z = Mathf.PerlinNoise (time+24, 876)-0.5f;
		return new Vector3(x,y,z)*2f;
	}

	/// <summary>
	/// Returns op's rotation as a quaternion in space's coordinates.
	/// </summary>
	public static Quaternion LocalQuaternion( Transform op, Transform space )
	{
		return Quaternion.LookRotation (space.InverseTransformDirection(op.forward),space.InverseTransformDirection (op.up));
	}

	/// <summary>
	/// Treats rot as a rotation in space space, and returns it expressed as a quaternion in world space.
	/// </summary>
	public static Quaternion GlobalQuaternion(Quaternion rot,Transform space)
	{
		return Quaternion.LookRotation( space.TransformDirection(rot*Vector3.forward), space.TransformDirection (rot*Vector3.up) );
	}

	public static Quaternion ScaleQuaternion( Quaternion rot, float amount )
	{
		if ( Mathf.Abs (amount - 0) < Mathf.Epsilon ) return Quaternion.identity;
		if ( Mathf.Abs (amount - 1) < Mathf.Epsilon) return rot;
		if ( Mathf.Abs (amount - -1) < Mathf.Epsilon) return Quaternion.Inverse (rot);
		float angle;
		Vector3 axis;
		rot.ToAngleAxis( out angle, out axis );
		angle *= amount;
		return Quaternion.AngleAxis ( angle, axis );
	}

	public static int Mod( int x, int m )
	{
		if(m==0)return 0;
		return (x%m + m)%m;
	}

	public static float Mod( float x, int m )
	{
		if(m==0)return 0;
		return (x%m + m)%m;
	}

	public static bool Sanity( bool assertion, string msg )
	{
		if ( assertion ) Debug.Log ( msg );
		return assertion;
	}


	public static Vector3 SplinePoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) 
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	public static Vector3 SplineDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) 
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}



	/// <summary> Returns value if it's further away from zero than lowShelf, else
	/// returns a signed lowShelf. </summary>
	public static float SignedMax( float value, float lowShelf )
		=> 	 Mathf.Max( Mathf.Abs( lowShelf ), Mathf.Abs( value ) ) 
			*Mathf.Sign( value );


	public static void DrawGizmosCircle( Transform transform, Vector3 localNormal, float radius )
		=> DrawGizmosCircle( transform, localNormal, radius, Color.blue );
	public static void DrawGizmosCircle( Transform transform, Vector3 localNormal, float radius, Color color )
	{
		var mat = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = color;
		DrawGizmosCircle( Vector3.zero, localNormal, radius );
		Gizmos.matrix = mat;
	}
	public static void DrawGizmosCircle(Vector3 origin, Vector3 normal, float radius, int steps = 24)
	{
		Vector3 p1, p2;
		float arc = 360f / steps;
		Quaternion rot = Quaternion.AngleAxis(arc, normal);
		Vector3 spokeDirection = Vector3.Cross (normal, Vector3.right);
		// check if the normal points as Vector3.right
		if ( Vector3.Angle(normal,Vector3.right) < Mathf.Epsilon) spokeDirection = Vector3.Cross (normal, Vector3.forward);
		if ( Vector3.Angle(normal,Vector3.right) >= 180 ) spokeDirection = Vector3.Cross (normal, Vector3.forward);
		//		if ( Mathf.Abs (spokeDirection.sqrMagnitude - 0) < Mathf.Epsilon) spokeDirection = Vector3.Cross (normal, Vector3.forward);

		spokeDirection=spokeDirection.normalized;

		p2 = origin + spokeDirection * radius;

		for (int i = 0; i < steps; i++)
		{
			p1 = p2;
			//Gizmos.DrawRay(topHingePosition.position, _gizmosCircleDir * gizmoRadius); // spokes
			spokeDirection = rot * spokeDirection;
			p2 = origin + spokeDirection * radius;
			Gizmos.DrawLine(p1, p2);
		}
	}


	/// <summary>
	/// Increases in chance the longer it takes to trigger
	/// </summary>
	public class CumulativeProbability
	{
		private float _frequency;
		private float _squaredDelay = 0f;

		public CumulativeProbability(float freq)
		{
			_frequency = freq;
		}

		/// <summary>
		/// Simulateously update the clock, set the desired frequency, and check for trigger.
		/// Run this only if you want to check every -delta- seconds! In all other cases, use Update() to update the internal clock, and Check() to check for trigger.
		/// </summary>
		public bool Check(float freq, float delta)
		{
			_frequency = freq;
			return Check (delta);
		}

		/// <summary>
		/// Updates the clock and checks for a trigger.
		/// Run this only if you want to check every -delta- seconds! In all other cases, use Update() to update the internal clock, and Check() to check for trigger.
		/// </summary>
		public bool Check(float delta)
		{
			Update (delta);
			return Check ();
		}

		/// <summary>
		/// Updates the internal clock.
		/// </summary>
		public void Update(float delta)
		{
			_squaredDelay += delta*delta; // TODO: this is WRONG. Just getting the mechanisms down
		}

		/// <summary>
		/// Checks for the probability to trigger.
		/// </summary>
		public bool Check()
		{
			if (Random.value<_squaredDelay*_frequency)
			{
				_squaredDelay = 0f;
				return true;
			}
			else return false;
		}
	}


	/// This function returns a point which is a projection from a point to a line.
	/// The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
	public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
	{		

		//get vector from point on line to point in space
		Vector3 linePointToPoint = point - linePoint;

		float t = Vector3.Dot(linePointToPoint, lineVec);

		return linePoint + lineVec * t;
	}

	/// This function returns a point which is a projection from a point to a line segment.
	/// If the projected point lies outside of the line segment, the projected point will 
	/// be clamped to the appropriate line edge.
	/// If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
	public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{

		Vector3 vector = linePoint2 - linePoint1;

		Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);

		int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

		//The projected point is on the line segment
		if(side == 0){

			return projectedPoint;
		}

		if(side == 1){

			return linePoint1;
		}

		if(side == 2){

			return linePoint2;
		}

		//output is invalid
		return Vector3.zero;
	}	


	/// This function finds out on which side of a line segment the point is located.
	/// The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
	/// the line segment, project it on the line using ProjectPointOnLine() first.
	/// Returns 0 if point is on the line segment.
	/// Returns 1 if point is outside of the line segment and located on the side of linePoint1.
	/// Returns 2 if point is outside of the line segment and located on the side of linePoint2.
	public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
	{

		Vector3 lineVec = linePoint2 - linePoint1;
		Vector3 pointVec = point - linePoint1;

		float dot = Vector3.Dot(pointVec, lineVec);

		//point is on side of linePoint2, compared to linePoint1
		if(dot > 0){

			//point is on the line segment
			if(pointVec.magnitude <= lineVec.magnitude){

				return 0;
			}

			//point is not on the line segment and it is on the side of linePoint2
			else{

				return 2;
			}
		}

		//Point is not on side of linePoint2, compared to linePoint1.
		//Point is not on the line segment and it is on the side of linePoint1.
		else{

			return 1;
		}
	}

}