using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    /// <summary> 
	/// Variation on the old PlasmaTether, more generalized. 
	/// Can be spawned in solo and connected to two transforms. Drives drawing the 
	/// plasma tethers between the various islands. <br/>
	/// Turn off enabled (or AutoUpdate) and manually call UpdateTetherGraphics 
	/// to prevent it from updating every frame. <br/>
	/// Can be parented anywhere.
	/// </summary>
    public class PlasmaTetherSolo : MonoBehaviour
    {
		const float _DEFAULTSTARTRADIUS = 3.83f;
		const float _DEFAULTENDRADIUS = 3.83f;

        [Range(2,32)] [SerializeField] int _bezierPoints = 8;
		[SerializeField] Transform _startTransform;
        [SerializeField] Transform _endTransform;
        [SerializeField] float _startRadius = _DEFAULTSTARTRADIUS;
        [SerializeField] float _endRadius = _DEFAULTENDRADIUS;

        [Tooltip("Height for the bezier middle point")]
        [SerializeField] float _controlPointYFactor = 0;

		public bool AutoUpdate{
			get => enabled;
			set => enabled = value;
		}


        LineRenderer _line;

		Vector3[] _positions;



		/// <summary> Creates a plasma tether between start and end transform, and returns the spawned
		/// PlasmaTetherSolo object. Currently uses a hardcoded radius for each island. <br/>
		/// autoUpdate denotes if the tether should update every frame (fi because the islands move).
		/// If not, you can call UpdateTetherGraphics() to update them.
		/// (You can always set AutoUpdate to true and false whenever needed)<br/>
		/// </summary>
		public static PlasmaTetherSolo Create( Transform start, Transform end, bool autoUpdate )
		{
			DefaultsManager.TriggerDefaulLoad();
			if( !PlasmaTetherSettings.Default )
			{
				Debug.LogError( "Please define a default Plasma Tether Prefab in the PlasmaTetherSettings object!" );	
				return null;
			}

			var tether = Instantiate( PlasmaTetherSettings.Default.Prefab ).GetComponent<PlasmaTetherSolo>();
			tether.Setup( start, end );
			tether.enabled = autoUpdate;
			return tether;
		}

		int[] _sizePerLOD = { 8, 5, 3, 2 };
		/// <summary> Hacky LOD settings between 0 and 3 </summary>
		public void SetLOD( int newLOD = 0 )
		{
			if( newLOD<0 || newLOD>_sizePerLOD.Length-1 ) return;
			RecalculateWith( _sizePerLOD[newLOD] );
		}


		/// <summary> Call this to update the tether graphics (fi when the islands move) </summary>
        public void UpdateTetherGraphics()
        {
            if( !_startTransform || !_endTransform )
            {
                _line.enabled = false;
                return;
            }
            else 
                _line.enabled = true;

			if( _startTransform.position.y < _endTransform.position.y )
			{
				(_startTransform,_endTransform) = (_endTransform,_startTransform);
				(_startRadius,_endRadius) = (_endRadius,_startRadius);
			}

			transform.position = _startTransform.position;

			_bezierPoints = Mathf.Min( 30, _bezierPoints );
            _positions = new Vector3[_bezierPoints];

            var dir = _endTransform.position - _startTransform.position;
            dir.y = 0;
            transform.position = _startTransform.position + dir.normalized *_startRadius;

            var p0 = Vector3.zero;
            dir = transform.position - _endTransform.position;
            dir.y = 0;
            var p2 = _endTransform.position + dir.normalized*_endRadius;

            var p1 = Vector3.Lerp(transform.position, p2, .5f);
            p1.y = Mathf.LerpUnclamped( p2.y, p0.y, _controlPointYFactor );

            // Debug.DrawRay( p1, Vector3.up, Color.yellow );
            // Debug.DrawRay( p2, Vector3.up, Color.black );

            p1 = transform.InverseTransformPoint( p1 );
            p2 = transform.InverseTransformPoint( p2 );
 
            for (int i = 0; i < _bezierPoints; i++)
            {
                float t = i / ((float)_bezierPoints-1);
                _positions[i] = Extensions.CalculateQuadraticBezierPoint(t, p0, p1, p2);
                // Debug.DrawRay( transform.TransformPoint( positions[i] ), Vector3.up, Color.red );
            }

            _line.positionCount = _bezierPoints;
            _line.SetPositions(_positions);
         }




		void Setup( Transform startTransform, Transform endTransform )
			=> Setup( startTransform, _DEFAULTSTARTRADIUS, endTransform, _DEFAULTENDRADIUS );
        void Setup( Transform startTransform, float startRadius, Transform endTransform, float endRadius )
        {
            // Debug.Log( startRadius );
			_startTransform = startTransform;
            _startRadius = startRadius;

            _endTransform = endTransform;
            _endRadius = endRadius;

            UpdateTetherGraphics();
        }




		void RecalculateWith( int newPointCount )
		{
			if( _bezierPoints == newPointCount ) return;
			_bezierPoints = newPointCount;
			UpdateTetherGraphics();
		}



        void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _line.useWorldSpace = false;
        }


        void Update()
        {
            UpdateTetherGraphics();
        }
    }
}