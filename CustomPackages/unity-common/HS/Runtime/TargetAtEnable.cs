using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS
{
	/// <summary>
	/// A basic targetting expression at enable. 
	/// </summary>
	public class TargetAtEnable : MonoBehaviour
	{
		void OnEnable()
		{
			var dir = -transform.position;
			dir.y = 0;
			// Unity throws an error: Look rotation viewing vector is zero, if the dir is Vector3.zero
			if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir,Vector3.up);
		}
	}
}