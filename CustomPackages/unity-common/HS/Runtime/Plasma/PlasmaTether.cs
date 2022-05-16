using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    /// <summary> Drives drawing the plasma tethers between the various islands. Turn off this.enabled 
    /// and manually call on UpdateTetherGraphics to prevent it from updating every frame.
    /// When running Setup, we'll be attaching to the given startTransform as a parent. </summary>
    public class PlasmaTether : MonoBehaviour
    {
		[Tooltip( "Keeps the line's cross-section horizontal" )]
		public bool Flat;

		[Header( "Start and End settings (autofilled)" )]
        public float ParentRadius = 5;
        public Transform Target;
        public float TargetRadius = 0;

		[Space]
		[SerializeField] Transform _endVisual;
		[SerializeField] bool _alignVisuals;

		[Space]
		[SerializeField] List<GameObject> _hideWhenOff;

		[Header( "Basic Bezier settings" )]
        [Range(2,32)]public int BezierPoints = 8;
        [Tooltip("Height for the bezier middle point")]
        public float ControlPointYFactor = 0;
		public int[] BezierPointsPerLOD =
            {
                8,
                5,
                3,
                2
            };


		ITetherAttachShape _startShape;
		ITetherAttachShape _endShape;


        LineRenderer _line;


		/// <summary> Tether Drawing Modules can subscribe to this to override how the tether is drawn </summary>
		public System.Action<Vector3,Vector3> OnDraw;
		/// <summary> Tether Drawing Modules can subscribe to this to override LOD changes </summary>
		public System.Action<int> OnLODChange;
		/// <summary> Tether Drawing Modules should set this to TRUE if they don't want the 
		/// tether object to reposition itself prior to adjusting the linerenderer </summary>
		public bool DontPrepLineRendering{get;set;}
		bool _hasDrawerModule => OnDraw != null && OnDraw.GetInvocationList().Length > 0;

		/// <summary> Smart Tether Setup: finds the necessary elements itself. Still contains access
		/// to legacy elements as well.
		/// </summary>
		public void Setup( GameObject startObject, GameObject endObject )
		{
			transform.SetParent( startObject.transform, true );

			_startShape = startObject?.GetComponentInChildren<ITetherAttachShape>();
			_endShape = endObject?.GetComponentInChildren<ITetherAttachShape>();
			// we need to make sure to -only- select shapes if they belong to the given platform, -not-
			// when they belong to a child of them
			// NB: ASSUMPTION: only new UserPlatformDriver are used, not legacy TeamSPaceDriver etc.
			if( _startShape != null && (_startShape as MonoBehaviour).GetComponentInParent<UserPlatformDriver>().gameObject != startObject ) 
				_startShape = null;
			if( _endShape != null && (_endShape as MonoBehaviour).GetComponentInParent<UserPlatformDriver>().gameObject != endObject ) 
				_endShape = null;
			Target = (_endShape as MonoBehaviour)?.transform ?? endObject?.transform;


			// find non-dynamic start radius
			var upd = startObject.GetComponent<UserPlatformDriver>();
			// FOR LEGACY:
			var teamsd = startObject.GetComponent<TeamSpaceDriver>();
			var tracksd = startObject.GetComponent<TrackSpaceDriver>();
			if( upd ) ParentRadius = upd.TetherRingRadius;
			else if( teamsd ) ParentRadius = teamsd.TetherRingRadius;
			else if( tracksd ) ParentRadius = tracksd.TetherRingRadius;

			if( endObject )
			{
				upd = endObject.GetComponent<UserPlatformDriver>();
				// FOR LEGACY:
				teamsd = endObject.GetComponent<TeamSpaceDriver>();
				tracksd = endObject.GetComponent<TrackSpaceDriver>();
				if( upd ) TargetRadius = upd.TetherRingRadius;
				else if( teamsd ) TargetRadius = teamsd.TetherRingRadius;
				else if( tracksd ) TargetRadius = tracksd.TetherRingRadius;
				else TargetRadius = 283; // LEGACY: was the radius of the world in original Momentum
			}
			else
			{
				TargetRadius = 283; // LEGACY: was the radius of the world in original Momentum
			}

			UpdateTetherGraphics();
		}


		public void RecalculateWithLOD( int lod )
		{
			if( _hasDrawerModule )
			{
				OnLODChange?.Invoke(lod);
				return;
			}

			var newPointCount = BezierPointsPerLOD[lod];
			if( BezierPoints == newPointCount ) return;
			BezierPoints = newPointCount;
			UpdateTetherGraphics();
		}

		Vector3[] _positions;
		[ContextMenu( "Update Tether" )]
        public void UpdateTetherGraphics()
        {
			TryGetReferences();

			if( _line ) _line.enabled = Target!=null;;
			foreach( var op in _hideWhenOff ) op.SetActive( Target!=null );
			if( _endVisual != null ) _endVisual.gameObject.SetActive( Target!=null );

			if( Target == null ) return;


			if( !DontPrepLineRendering )
			{
				if( Flat ) transform.rotation = Quaternion.LookRotation( Vector3.down );

				if( _startShape != null ) transform.position = _startShape.GetWorldPos( Target.position );
				else
				{
					var dir = Target.position - transform.parent.position;
					dir.y = 0;
					transform.position = transform.parent.position + dir.normalized *ParentRadius;
				}
			}

			var startPoint = Vector3.zero;

			Vector3 endPoint;
			if( _endShape != null ) endPoint = _endShape.GetWorldPos( _startShape != null ? (_startShape as MonoBehaviour).transform.position : transform.parent.position );
			else
			{
				var dir = transform.position - Target.position;
				dir.y = 0;
				endPoint = Target.position + dir.normalized*TargetRadius;
			}
			endPoint = transform.InverseTransformPoint( endPoint );


			// if a module added itself, we should use that for drawing
			if( _hasDrawerModule ) OnDraw( startPoint, endPoint );
			// else we stick to basic bezier shapes
			else
			{
				BezierPoints = Mathf.Min( 30, BezierPoints );
				_positions = new Vector3[BezierPoints];


				var midPoint = Vector3.Lerp(transform.position, transform.TransformPoint(endPoint), .5f);
				midPoint.y = Mathf.LerpUnclamped( transform.TransformPoint(endPoint).y, transform.position.y, ControlPointYFactor );
				midPoint = transform.InverseTransformPoint( midPoint );


				for (int i = 0; i < BezierPoints; i++)
				{
					float t = i / ((float)BezierPoints-1);
					_positions[i] = Extensions.CalculateQuadraticBezierPoint(t, startPoint, midPoint, endPoint);
				}


				_line.positionCount = BezierPoints;
				_line.SetPositions(_positions);
			}

			if( _endVisual )
			{
				_endVisual.localPosition = endPoint;
				if( _alignVisuals ) _endVisual.localRotation = Quaternion.LookRotation(startPoint-endPoint,Vector3.up);
			}
         }


		void Awake()
		{
			TryGetReferences();
			_line.enabled = false;
		}


        void TryGetReferences()
        {
			if( _line ) return;
            _line = GetComponent<LineRenderer>();
			if( !_line ) return;
            _line.useWorldSpace = false;
        }
    }
}