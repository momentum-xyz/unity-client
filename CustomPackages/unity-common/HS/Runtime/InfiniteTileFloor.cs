using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;


namespace HS
{
	public class InfiniteTileFloor : MonoBehaviour
	{
		public float Distance = 450;
		public float TileSize = 450;


		GameObject _source;
		Transform _cam;
		Cell _lastCell;
		HashSet<GameObject> _cells = new HashSet<GameObject>();
		bool _enforceRegen;


		void Awake()
		{
			_source = transform.GetChild(0).gameObject;
			_source.SetActive( false );
		}


		void OnEnable()
		{
			_enforceRegen = true;
		}


		void Update()
		{
			if( !_cam ) _cam = Camera.main?.transform;
			if( !_cam ) return;

			var newCell = new Cell(Vector3.Scale(_cam.position,new Vector3(1,0,1))/TileSize);
			if( _enforceRegen || newCell != _lastCell ) Regen( newCell );
			_lastCell = newCell;
		}


		void Regen( Cell cell )
		{
			var spread = (int)Mathf.Round( Distance/TileSize );
			spread = Mathf.Max(1,spread);
			var width = 1+(spread-1)*2;
			var count = Mathf.Pow( width, 2 );


			// make sure there are enough gameobjects (poolied system)
			if( _cells == null ) _cells = new HashSet<GameObject>();
			while( _cells.Count < count )
			{
				var newOp = _source.SpawnSibling();
				newOp.SetActive( true );
				_cells.Add( newOp );
			}
			while( _cells.Count > count)
			{
				var op = _cells.FirstOrDefault();
				_cells.Remove( op );
				Destroy( op );
			}


			int x = 0;
			int z = 0;
			foreach( var tile in _cells )
			{
				// Debug.Log( $"cell at ({x},{z})" );
				tile.transform.position = 
					(
						cell.Value
						+ new Vector3(x,0,z)
						- new Vector3(1,0,1)*(spread-1)
					)
					*TileSize 
					;
				if( x == width-1 ) z++;
				x = (x+1)%width;
			}


			_enforceRegen = false;
			return;
		}



		struct Cell
		{
			public Vector3 Value;
			public Cell( Vector3 point )
			{
				Value = new Vector3(
					Mathf.Round(point.x),	
					Mathf.Round(point.y),	
					Mathf.Round(point.z)	
				);
			}

			public static bool operator ==( Cell one, Cell two ) => one.Value==two.Value;
			public static bool operator !=( Cell one, Cell two ) => one.Value!=two.Value;
		}
	}
}