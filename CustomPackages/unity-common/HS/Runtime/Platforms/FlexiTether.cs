using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nextensions;


namespace HS
{
	/// <summary>
	/// A Tether module that works with skinned meshes.
	/// </summary>
	public class FlexiTether : MonoBehaviour
	{
		[SerializeField][Range(0,1)]float _midRotation = 0.7f;
		PlasmaTether _tether;

		Transform _start, _mid, _end;

		void Awake()
		{
			_start = transform.FindByTaggedName( "@startJoint" );
			_mid = transform.FindByTaggedName( "@midJoint" );
			_end = transform.FindByTaggedName( "@endJoint" );
		}

		void OnEnable()
		{
			if( !_tether ) _tether = GetComponent<PlasmaTether>();
			if( !_tether ) return;
			_tether.DontPrepLineRendering = true;
			_tether.OnDraw += Draw;
		}

		void OnDisable()
		{
			if( !_tether ) return;
			_tether.OnDraw -= Draw;
		}


		void Draw( Vector3 start, Vector3 end )
		{
			_end.position = transform.TransformPoint( end );
			_end.rotation = Quaternion.LookRotation( Vector3.Scale(transform.TransformDirection(end-start),new Vector3(1,0,1) ), Vector3.up );
			var midPosition = (transform.TransformPoint( start )+_end.position)/2;
			midPosition.y = _end.position.y;
			_mid.position = midPosition;
			_mid.rotation = Quaternion.Lerp( _start.rotation, _end.rotation, _midRotation );
		}
	}
}