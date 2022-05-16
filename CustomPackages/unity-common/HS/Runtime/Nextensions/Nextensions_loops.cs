using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Nextensions
{
	public partial class Nextensions
	{
		public static void ForEach<T>( this T[] array, System.Action<int> action )
		{
			for(int i=0;i<array.Length;i++)
				action?.Invoke(i);
		}

		// bit overwrought this, you can call it by, for example:
		// 	_positions.ForEach( (ref Vector3 p,int idx)=>
		// 		Debug.Log( $"{idx}: {p}" )
		// 	);
		public delegate void ActionRef<T>(ref T elm, int idx);
		public static void ForEach<T>( this T[] array, ActionRef<T> action )
		{
			for(int i=0;i<array.Length;i++)
				action?.Invoke(ref array[i],i);
		}
	}
}
