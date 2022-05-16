using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

namespace Nextensions
{
	public static partial class Nextensions
	{
		//
		// transform helpers
		//
		public static void Zero( this Transform transform )
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}


		public static void Pose( this Transform transform, Transform target )
		{
			transform.position = target.position;
			transform.rotation = target.rotation;
		}


		public static void Mimic( this Transform transform, Transform target )
		{
			var p = transform.parent;
			transform.SetParent( target );
			transform.Zero();
			transform.SetParent( p, true );
		}


		public static Vector3 EulerWrap( this Vector3 euler )
		{
			while( euler.x> 180 ) euler.x-=360;
			while( euler.x<-180 ) euler.x+=360;
			while( euler.y> 180 ) euler.y-=360;
			while( euler.y<-180 ) euler.y+=360;
			while( euler.z> 180 ) euler.z-=360;
			while( euler.z<-180 ) euler.z+=360;
			return euler;
		}


		//
		// Vector helpers
		//
		public static Vector3 Wrap( this Vector3 p, float radius )
		{
			if( p.x> radius ) p.x-=2*radius;
			if( p.x<-radius ) p.x+=2*radius;
			if( p.y> radius ) p.y-=2*radius;
			if( p.y<-radius ) p.y+=2*radius;
			if( p.z> radius ) p.z-=2*radius;
			if( p.z<-radius ) p.z+=2*radius;
			return p;
		}


		public static Vector3 Rotate( this Vector3 v, float angle, Vector3 axis ) =>
			Quaternion.AngleAxis( angle, axis ) *v;


		public static Vector3 ScaleBy( this Vector3 v, float x, float y, float z ) =>
			Vector3.Scale( v, new Vector3(x,y,z) );

		//
		// component helpers
		//
		public static T CreateOrAddComponent<T>( this GameObject op ) where T: Component
			=> op.transform.CreateOrAddComponent<T>();
		public static T CreateOrAddComponent<T>( this Component component ) where T : Component
		{
			T result = component.GetComponent<T>();
			if( result != null ) return result;
			return component.gameObject.AddComponent<T>();
		}


		//
		// search and tag helpers
		//
		public static string Tag( this string str, string tag )
		{
			if( str.ToUpper().Contains(tag.ToUpper() ) ) return str;
			if( tag.StartsWith("@") == false ) tag = $"@{tag}";
			return $"{str}_{tag}";
		}

		public static GameObject FindByTaggedName( this GameObject op, string tag ) =>
			op.transform.FindByTaggedName( tag )?.gameObject;
		public static T FindByTaggedName<T>( this GameObject op, string tag ) where T: Component =>
			op.transform.FindByTaggedName<T>( tag );
		public static Transform FindByTaggedName( this Component component, string tag ) =>
			component.GetComponentsInChildren<Transform>(true).FirstOrDefault( a=> a.gameObject.name.ToUpper().Contains(tag.ToUpper()) );
		public static T FindByTaggedName<T>( this Component component, string tag ) where T : Component =>
			component.GetComponentsInChildren<Transform>(true).FirstOrDefault( a=> a.gameObject.name.ToUpper().Contains(tag.ToUpper()) )?.GetComponent<T>();
		public static IEnumerable<Transform> FindByTaggedNameAll( this Component component, string tag ) =>
			component.GetComponentsInChildren<Transform>(true).Where( a=> a.gameObject.name.ToUpper().Contains(tag.ToUpper()) );
		public static IEnumerable<T> FindByTaggedNameAll<T>( this Component component, string tag ) where T : Component =>
			component.GetComponentsInChildren<T>(true).Where( a=> a.gameObject.name.ToUpper().Contains(tag.ToUpper()) );
		
		public static string GetTag( this string str )
		{
			var matches = Regex.Match( str, @"(@[a-zA-Z0-9_]+)" );
			if( matches.Success ) return matches.Groups[1].Value;
			else return "";
		}

		public static string GetCleanTag( this string str )
		{
			var matches = Regex.Match( str, @"@([a-zA-Z0-9_]+)" );
			if( matches.Success ) return matches.Groups[1].Value;
			else return "";
		}

		public static string SplitTag( this string str, string startOfTag )
		{
			var result = Regex.Replace( str.GetTag().ToUpper(), startOfTag.ToUpper(), "" );
			result = Regex.Replace( result, @"_+.*", "" ); // an underscore ends a tag
			if( result.Length == 0 ) return null;
			else return "@"+result;
		}

		//
		// C# type helpers
		//
		public static HashSet<T> ToHashSet<T>( this T[] array ) => new HashSet<T>(array);

		public static T RandomPull<T>( this List<T> list )
		{
			var result = list[Random.Range(0,list.Count)];
			list.Remove(result);
			return result;
		}

		public static T Pull<T>( this List<T> list )
		{
			var result = list[0];
			list.RemoveAt(0);
			return result;
		}


		public static bool IsEmpty( this string str )
			=> str==null || str.Length == 0 || Regex.IsMatch(str,@"^\s*$");
		public static bool IsPresent( this string str ) => !str.IsEmpty();


		public static string SplitAtCapitals( this string value )
		{
			var result = "";
			foreach( var letter in value )
				result += (System.Char.IsUpper(letter)?" ":"") + letter;
			result = result.Trim();
			return result;
		}


		public static string[] CSVFields( this string str )
		{
			var result = str.Split( ',' ).ToList();
			var startMatch = "^\\s*\"";
			var endMatch = "\"\\s*$";
			int mergeIdx = -1;
			var remove = new HashSet<string>();
			string dbg;
			for( var i = 0; i < result.Count; i++ )
			{
				dbg = $"EVAL:[{result[i]}]";
				if( mergeIdx > -1 )
				{
					result[mergeIdx]+= $",{Regex.Replace(result[i],endMatch,"")}";
					dbg += $": merging backwards into {mergeIdx}";
					remove.Add(result[i]);
				}
				if( Regex.IsMatch(result[i],endMatch) ) //result[i].EndsWith( @"""" ) )
				{
					dbg += $": result: [{result[mergeIdx]}]"; 
					mergeIdx = -1;
				}
				else if( Regex.IsMatch(result[i],startMatch) ) //result[i].StartsWith( @"""" ) )
				{
					dbg += $": starting merge at {i}"; 
					result[i]=Regex.Replace(result[i],startMatch,"");
					mergeIdx = i;
				}
				Debug.Log( dbg );
			}
			dbg = $"Removed from {result.Count}";
			result.RemoveAll( s=> remove.Contains(s) );//Regex.IsMatch( s, endMatch ) );
			dbg += $" to {result.Count}";
			Debug.Log( dbg );
			return result.ToArray();
		}


		public static string ToClockString( this float seconds ) => $"{(int)(seconds/60)%60:00}:{(int)seconds%60:00}:{(int)(seconds*100)%100:00}";


		public static void SetOrAdd<Tkey,Tvalue>( this Dictionary<Tkey,Tvalue> dict, Tkey key, Tvalue value )
		{
			if( dict.ContainsKey(key) ) dict[key] = value;
			else dict.Add( key, value );
		}
		public static Tvalue GetOrAdd<Tkey,Tvalue>( this Dictionary<Tkey,Tvalue> dict, Tkey key ) where Tvalue : class, new()
		{
			if( dict.ContainsKey(key) ) return dict[key];
			Tvalue result = new Tvalue();
			dict.Add( key, result );
			return result;
		}

		public static TValue Get<T,TValue>( this Dictionary<T,TValue> dict, T key )
		{
			if( dict.ContainsKey(key) ) return dict[key];
			else return default(TValue);
		}

		public static T Get<T>( this List<T> list, int idx )
		{
			if( idx < list.Count ) return list[idx];
			else return default(T);
		}


		public static T PickOne<T>( this T[] a ) => a.Length > 0 ? a[Random.Range(0,a.Length)] : default;
		public static T PickOne<T>( this List<T> list ) => list.Count > 0 ? list[Random.Range(0,list.Count)] : default;


		//
		// spawn helpers
		//
		public static GameObject Spawn( this GameObject prefab )
		{
			var newOp = GameObject.Instantiate( prefab );
			newOp.SetActive( true );
			return newOp;
		}
		public static GameObject SpawnAt( this GameObject prefab, GameObject other, bool scale = false )
			=> prefab.SpawnAt( other.transform, scale );
		public static GameObject SpawnAt( this GameObject prefab, Component other, bool scale = false )
		{
			var newOp = Spawn( prefab );
			if( scale ) newOp.transform.Mimic( other.transform );
			else newOp.transform.Pose( other.transform );
			newOp.transform.position = other.transform.position;
			newOp.transform.rotation = other.transform.rotation;
			return newOp;
		}
		public static GameObject SpawnSibling( this GameObject source ) =>
			SpawnInside( source, source.transform.parent );
		public static GameObject SpawnInside( this GameObject prefab, Transform transform )
		{
			var newOp = GameObject.Instantiate( prefab );
			newOp.transform.SetParent(transform);
			newOp.transform.Zero();
			newOp.SetActive( true );
			return newOp;
		}
	}
}