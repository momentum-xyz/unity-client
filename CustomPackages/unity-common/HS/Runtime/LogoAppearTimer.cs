using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace HS
{
	public class LogoAppearTimer : MonoBehaviour
	{
		public AudioSource Audio;
		public float Offset = 0;

		public bool DoRecordTimes;
		public KeyCode RecordKey = KeyCode.KeypadMultiply;
		public List<float> Times;


		Material _mat;


		float _value = 0;


		void OnEnable()
		{
			if( !DoRecordTimes ) StartCoroutine( RunPlayback() );
		}


		void Start()
		{
			if( DoRecordTimes ) Times.Clear();
			_mat = GetComponent<MeshRenderer>().material;
			_mat.SetFloat( "_Envelope", 0 );
		}


		IEnumerator RunPlayback()
		{
			yield return new WaitUntil( ()=> Audio!=null && Audio.isPlaying );
			_mat.SetFloat( "_Envelope", 0 );
			_value = 0;
			int idx = 0;
			while( Audio!=null && Audio.isPlaying && idx < Times.Count )
			{
				if( Audio.time + Offset >= Times[idx] )
				{
					_value += 0.05f;
					idx++;
					StartCoroutine( UpdateMaterial() );
				}
				yield return null;
			}
		}


		IEnumerator UpdateMaterial()
		{
			while( Mathf.Abs(_mat.GetFloat( "_Envelope")-_value)>0.001f )
			{
				_mat.SetFloat( "_Envelope", Mathf.Lerp( _mat.GetFloat( "_Envelope" ), _value, Time.unscaledDeltaTime * 3 ) );
				yield return null;
			}
		}


		void Update()
		{
			if( DoRecordTimes && Audio.isPlaying && Input.GetKeyDown( RecordKey ) )
			{
				Debug.Log( "RECORD" );
				Times.Add( Audio.time );
				_value += 0.05f;
				_mat.SetFloat( "_Envelope", _value );
			}
		}
	}
}