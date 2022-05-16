using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HS
{
	public class TickerDriver : MonoBehaviour
	{
		[Header( "Local references" )]
		[SerializeField] RadialCloner TopTicker;
		[SerializeField] RadialCloner BottomTicker;

		public bool isSetup = false;

		TMP_Text[] _topTickerClocks;
		TMP_Text[] _topTickerLabels;
		TMP_Text[] _bottomTickerClocks;
		TMP_Text[] _bottomTickerLabels;
		RawImage[] _topTickerScreens;
		RawImage[] _bottomTickerScreens;


		/// <summary> Set the top (big) ticker clock to given DateTime. </summary>
		public void SetTopTicker( System.DateTimeOffset time ) =>
			SetTopTicker( $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}" );
		/// <summary> Set the bottom (small) ticker clock to given DateTime. </summary>
		public void SetBottomTicker( System.DateTime time ) =>
			SetBottomTicker( $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}" );
		/// <summary> Set the top (big) ticker clock to given string. </summary>
		public void SetTopTicker( string newString )
		{
			for( int i = 0; i < _topTickerClocks.Length; i++ ) 
				_topTickerClocks[i].text = newString;
		}
		/// <summary> Set the bottom (small) ticker clock to given string. </summary>
		public void SetBottomTicker( string newString )
		{
			for( int i = 0; i < _bottomTickerClocks.Length; i++ )
				 _bottomTickerClocks[i].text = newString;
		}

		/// <summary> Set the top (big) ticker clock label to given string. </summary>
		public void SetTopLabel( string newString )
		{
			for( int i = 0; i < _topTickerClocks.Length; i++ ) 
				_topTickerLabels[i].text = newString;
		}
		/// <summary> Set the bottom (smaller) ticker clock label to given string. </summary>
		public void SetBottomLabel( string newString )
		{
			for( int i = 0; i < _bottomTickerClocks.Length; i++ )
				 _bottomTickerLabels[i].text = newString;
		}

		/// <summary> Set the top (big) ticker screen to the given (16x9) texture </summary>
		public void SetTopScreen( Texture2D newScreen )
		{
			foreach(var img in _topTickerScreens)
				img.texture = newScreen;
		}
		/// <summary> Set the bottom (small) ticker screen to the given (16x9) texture </summary>
		public void SetBottomScreen( Texture2D newScreen )
		{
			foreach(var img in _bottomTickerScreens)
				img.texture = newScreen;
		}

		IEnumerator Start()
		{
			yield return new WaitUntil( ()=> TopTicker.IsSetup && BottomTicker.IsSetup );
			_topTickerClocks = TopTicker.GetComponentsInChildren<TMP_Text>( true ).Where<TMP_Text>( elm => elm.gameObject.name == "[CLOCK]").ToArray();
			_topTickerLabels = TopTicker.GetComponentsInChildren<TMP_Text>( true ).Where<TMP_Text>( elm => elm.gameObject.name == "[LABEL]").ToArray();
			_topTickerScreens = TopTicker.GetComponentsInChildren<RawImage>(true).Where<RawImage>(elm=>elm.gameObject.name.ToUpper().Contains("[IMAGE]")).ToArray();
			_bottomTickerClocks = BottomTicker.GetComponentsInChildren<TMP_Text>( true ).Where<TMP_Text>( elm => elm.gameObject.name == "[CLOCK]").ToArray();
			_bottomTickerLabels = BottomTicker.GetComponentsInChildren<TMP_Text>( true ).Where<TMP_Text>( elm => elm.gameObject.name == "[LABEL]").ToArray();
			_bottomTickerScreens = BottomTicker.GetComponentsInChildren<RawImage>(true).Where<RawImage>(elm=>elm.gameObject.name.ToUpper().Contains("[IMAGE]")).ToArray();
			SetupCompassLabels();
			isSetup = true;
		}


		void SetupCompassLabels()
		{

			var compassDirs = 
					GetComponentsInChildren<TMP_Text>( true )
					.Where<TMP_Text>( elm=>elm.name.ToUpper().Contains( "[COMPASS]" ))
					;
			foreach( var elm in compassDirs )
			{
				// @REMOVE: the below was way off mark, keeping the algo here for posterity. I checked that
				// the original momentum clocks still look the same. It can go after making sure.
				// if( Legacy )
				// {
				// 	var angle = Vector3.SignedAngle( transform.forward, transform.InverseTransformPoint(elm.transform.position), Vector3.up );
				// 	int idx = Mathf.RoundToInt(angle/360f *_compassDirections +_compassRoundingValue) +_compassOffset ;
				// 	elm.text = 
				// 		"S|ssw|SW|sww|W|nww|NW|nnw|N|nne|NE|nee|E|see|SE|sse|S"
				// 		.Split( '|' )
				// 		[idx];
				// }
				// else

				// fixed
				var angle = Vector3.SignedAngle( 
								Vector3.forward, 
								Vector3.Scale( transform.InverseTransformPoint(elm.transform.position), new Vector3(1,0,1) ), 
								Vector3.up 
							);
				angle = (angle+360)%360;
				var idx = ((int)(angle/360f*16f+0.5f))%16;
				elm.text = 
					"N,nne,NE,nee,E,see,SE,sse,S,ssw,SW,sww,W,nww,NW,nnw"
					.Split(',')
					[idx];
			}
		}
	}
}