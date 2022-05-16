using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	public interface ITetherAttachShape
	{
		public Vector3 GetWorldPos( Vector3 origin );
	}
}