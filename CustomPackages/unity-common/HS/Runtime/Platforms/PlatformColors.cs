using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;



namespace HS
{
	/// <summary> Runs through all child renderers (!) and sees if the given materials appear, then 
	/// changes them. </summary>
	public class PlatformColors : MonoBehaviour
	{
		[Tooltip( "Floormaterials are assumed to have a shader with color properties for _Albedo1, and _NeonTint" )]
		[SerializeField] List<Material> _floorMaterials;
		[SerializeField] bool FindFloorMaterials;
		[Tooltip( "TintMaterials pickup the main color (nr 0)")]
		[SerializeField] List<Material> _tintMaterials;
		[SerializeField] bool FindTintMaterials;
		[Tooltip( "TintMaterials pickup the accent color (nr 1)")]
		[SerializeField] List<Material> _accentMaterials;
		[SerializeField] bool FindAccentMaterials;


		UserPlatformDriver _driver;
	

		string[] _tintProperties = new string[]{
			"_BaseColor",
			"_Tint",
			"_MainColor"
		};


		public void Set( Color[] colors )
		{
			if( Application.isPlaying == false )
			{
				Debug.LogError( "We shouldn't do this in edit modde, as it will change SharedMaterials all over the place" );
				return;
			}


			// first we figure out which live materials are instances of the given shared materials
			var liveMats = new Dictionary<string,HashSet<Material>>();
			foreach( var r in GetComponentsInChildren<Renderer>(true) )
			{
				foreach( var mat in r.materials )
				{
					var name = mat.name;
					name = name.Replace(" (Instance)","");
					if(liveMats.ContainsKey(name)==false) liveMats.Add(name,new HashSet<Material>());
					liveMats[name].Add(mat);
				}
			}

			foreach(var mat in _floorMaterials )
				if(liveMats.ContainsKey(mat.name))
				{
					foreach(var m in liveMats[mat.name]) m.SetColor("_Albedo1",colors[0]);
					foreach(var m in liveMats[mat.name]) m.SetColor("_NeonTint",colors[1]);
				}

			foreach(var mat in _tintMaterials)
				if(liveMats.ContainsKey(mat.name))
					foreach(var prop in _tintProperties)
						foreach(var m in liveMats[mat.name])
							if(m.HasProperty(prop))
								m.SetColor(prop,colors[0]);//*mat.GetColor(prop));

			foreach(var mat in _accentMaterials)
				if(liveMats.ContainsKey(mat.name))
					foreach(var prop in _tintProperties)
						foreach(var m in liveMats[mat.name])
							if(m.HasProperty(prop))
								m.SetColor(prop,colors[1]);//*mat.GetColor(prop));
		}

		void OnValidate()
		{
			if( !Application.isEditor || Application.isPlaying ) return;
			if( FindFloorMaterials )
				foreach( var r in GetComponentsInChildren<Renderer>() )
					foreach( var m in r.sharedMaterials )
						if( !_floorMaterials.Contains(m) )
							if( m.HasProperty( "_Albedo1" ) || m.HasProperty( "_Neon" ) )
								_floorMaterials.Add(m);

			if( FindTintMaterials )
				foreach( var r in GetComponentsInChildren<Renderer>() )
					foreach( var m in r.sharedMaterials )
						if( !_tintMaterials.Contains(m) && IsTintable(m) )
							_tintMaterials.Add(m);

			if( FindAccentMaterials )
				foreach( var r in GetComponentsInChildren<Renderer>() )
					foreach( var m in r.sharedMaterials )
						if( !_accentMaterials.Contains(m) && IsTintable(m) )
							_accentMaterials.Add(m);

			FindFloorMaterials = false;
			FindTintMaterials = false;
			FindAccentMaterials = false;
		}


		bool IsTintable( Material m )
		{
			foreach(var prop in _tintProperties)
				if( m.HasProperty(prop) ) return true;
			return false;
		}


		void Awake()
		{
			_driver = GetComponentInParent<UserPlatformDriver>();
			_driver?.Colorables.Add(this);
		}


		void OnDestroy()
		{
			_driver?.Colorables.Remove(this);
		}
	}
}