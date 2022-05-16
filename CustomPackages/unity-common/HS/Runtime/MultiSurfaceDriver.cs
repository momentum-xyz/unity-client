using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Nextensions;

namespace HS
{
	public class MultiSurfaceDriver : MonoBehaviour
	{
		public List<TaggedSurface<TMP_Text>> Labels;
		public List<TaggedSurface<Renderer>> Screens;
		public List<TaggedSurface<TMP_Text>> Clocks;



		[ContextMenu( "Setup" )]
		void Setup() => Setup(transform);
		/// <summary> Have MultiSurfaceSetupDriver search own its own tagged surfaces within the given root object. </summary>
		public void Setup( Transform root )
		{
			AddStuff( Labels, root, "@Label" );
			AddStuff( Screens, root, "@Screen" );
			AddStuff( Clocks, root, "@Clock" );
		}


		public void SetLabel( string labelText, string tag = "" ) { 
			foreach( var op in Labels ) if ( tag==""||tag.ToUpper()==op.Tag.ToUpper() ) op.Surface.text = labelText; 
		}
		public void SetClock(System.TimeSpan time, string tag= "")
        {
			foreach (var op in Clocks) if (tag == "" || tag.ToUpper() == op.Tag.ToUpper()) op.Surface.text = $"{time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";
		}

		public void SetClock( System.DateTimeOffset time, string tag = "" ) { 
			foreach( var op in Clocks ) if ( tag==""||tag.ToUpper()==op.Tag.ToUpper() ) op.Surface.text = $"{time.Hour:00}:{time.Minute:00}:{time.Second:00}"; 
		}
		public void SetClock( string timeString, string tag = "" ) { 
			foreach( var op in Clocks ) if ( tag==""||tag.ToUpper()==op.Tag.ToUpper() ) op.Surface.text = timeString; 
		}
		public void SetScreen( Texture2D texture, string tag = "" ) { 
			foreach( var op in Screens ) if ( tag==""||tag.ToUpper()==op.Tag.ToUpper() ) 
				foreach( var prop in new[]{"_BaseMap","_MainTex"} )
					op.Surface.material.SetTexture( prop, texture );
		}


		void AddStuff<T>( List<TaggedSurface<T>> collection, Transform root, string tag ) where T : Component
		{
			foreach( var elm in root.FindByTaggedNameAll<T>( tag ) )
			{
				if( collection.FirstOrDefault( a=> a.Surface == elm ) == null ) // not added yet!
					collection.Add( new TaggedSurface<T>( elm, elm.gameObject.name.GetTag().SplitTag(tag) ) ); 
			}
		}


		[System.Serializable]
		public class TaggedSurface<T>
		{
			public string Tag;
			public T Surface;
			public TaggedSurface( T element, string tag )
			{
				Tag = tag;
				Surface = element;
			}
		}
	}
}