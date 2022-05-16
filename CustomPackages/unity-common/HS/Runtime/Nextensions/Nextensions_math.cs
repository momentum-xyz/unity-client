using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nextensions
{
	public static partial class Nextensions
	{
		public static float Abs(this float value) => Mathf.Abs(value);
		public static int Int(this float value)=>(int)value;
		public static float Max(this float value, float max)=>Mathf.Max(value,max);
		public static int Max(this int value, int max)=>Mathf.Max(value,max);



	}
}