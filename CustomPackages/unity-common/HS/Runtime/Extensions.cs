using UnityEngine;
using System.Text.RegularExpressions;

public static class Extensions
{
	public static Vector3 ZeroNan( this Vector3 v )
		=> v.IsNan() ? Vector3.zero : v;
	public static Quaternion ZeroNaN( this Quaternion q )
		=> q.IsNan() ? Quaternion.identity : q;
	public static bool IsNan( this Vector3 v )
		=> float.IsNaN(v.x)||float.IsNaN(v.y)||float.IsNaN(v.z);
	public static bool IsNan( this Quaternion q )
		=> float.IsNaN(q.x)||float.IsNaN(q.y)||float.IsNaN(q.z)||float.IsNaN(q.w);

	public static void Zero( this Transform transform )
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static void Place( this Transform transform, Transform target ) =>
		transform.position = target.position;
	public static void Place( this Transform transform, Vector3 targetLocation ) =>
		transform.position = targetLocation;

	public static void OrientTowards( this Transform transform, Vector3 targetLocation )
	{
		var dir = targetLocation - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
	}
	public static void OrientTowards( this Transform transform, Transform target )
	{
		var dir = target.position - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
	}

	public static T One<T>( this T[] members ) 
	{
		if( members == null ) return default(T);
		if( members.Length == 1 ) return members[0];
		return members[Random.Range(0,members.Length)];
	}
	public static (T,T) Two<T>( this T[] members ) 
	{
		if( members == null ) return (default(T),default(T));
		if( members.Length == 1 ) return (members[0],members[0]);
		if( members.Length == 2 ) return (members[0],members[1]);
		T t1 = members[Random.Range(0,members.Length)];
		T t2 = t1;
		while( t1.Equals(t2) ) t2 = members[Random.Range(0,members.Length)];
		return (t1,t2);
	}


	/// <summary> Attempts to evaluate a string for a ratio notation,
	/// like 1x2, or 5x3, or 16x9. Returns it like a float.
	/// If not found, returns the default of 4/3 (or the default provided) </summary>
	public static float PickupRatio( this string name, float defaultRatio = 3f/4f )
	{
		var match = Regex.Match( name, @"(\d+)x(\d+)" );
		if( match.Success )
		{
			// Debug.Log( $"Found {match.Groups[1]} and {match.Groups[2]} in {name}" );
			return float.Parse(match.Groups[1].Value)/float.Parse(match.Groups[2].Value);
		}
		return defaultRatio;
	}

	/// <summary> Attempts to evaluate a string for a ratio notation,
	/// like 1x2, or 5x3, or 16x9., and return the factors as a tuple.
	/// If not found, returns 4x3</summary>
	public static (int a, int b) PickupRatioFactors( this string name ) => PickupRatioFactors(name,3,4);
	/// <summary> Attempts to evaluate a string for a ratio notation,
	/// like 1x2, or 5x3, or 16x9., and return the factors as a tuple.
	/// If not found, returns the given default. </summary>
	public static (int a, int b) PickupRatioFactors( this string name, int defaultRatioA, int defaultRatioB )
	{
		var match = Regex.Match( name, @"(\d+)x(\d+)" );
		if( match.Success )
		{
			// Debug.Log( $"Found {match.Groups[1]} and {match.Groups[2]} in {name}" );
			return (int.Parse(match.Groups[1].Value),int.Parse(match.Groups[2].Value));
		}
		return (defaultRatioA,defaultRatioB);
	}

	/// <summary>
	/// Calculate the quadratic bezier point with the formula: 
	/// B(1-t) = (1-t)2P0 + 2(1-t)tP1 + t2P2
	/// </summary>
	public static Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		Vector3 p = uu * p0;
		p += 2 * u * t * p1;
		p += tt * p2;
		return p;
	}
}
