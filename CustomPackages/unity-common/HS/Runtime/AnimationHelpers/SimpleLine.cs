using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
	public class SimpleLine : MonoBehaviour
	{
		[SerializeField] Transform _targetEnd;
		[Header( "optional" )]
		[SerializeField] Transform _targetSelf;

		LineRenderer _line;
		Vector3 _lastPos;
		Vector3 _lastTargetPos;

		Vector3 _selfPos => _targetSelf == null ? transform.position : _targetSelf.position;


		void Awake()
		{
			_line = GetComponent<LineRenderer>();
		}


		void OnEnable()
		{
			_lastPos = _selfPos;
			_lastTargetPos = _targetEnd.position;
		}


		[ContextMenu( "Update Now")]
		void UpdateNow()
		{
			if( !_line ) _line = GetComponent<LineRenderer>();
			if( !_line ) return;
			_line.SetPosition( 0, transform.InverseTransformPoint( _selfPos ) );
			_line.SetPosition( 1, transform.InverseTransformPoint( _targetEnd.position ) );
		}


		void LateUpdate()
		{
			if( _lastPos != _selfPos || _lastTargetPos != _targetEnd.position ) UpdateNow();
			_lastPos = _selfPos;
			_lastTargetPos = _targetEnd.position;
		}
	}
}