using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
	/// <summary> Simply forces a qualitysetter call so that this camera picks up correct Quality settings </summary>
	public class QualityCameraTriggerer : MonoBehaviour
	{
		void OnEnable()
		{
			QualitySetter.SetQuality( QualitySetter.CurrentQuality );
		}
	}
}
