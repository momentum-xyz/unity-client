using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class BlockStateVisualsDriver : MonoBehaviour, IWorldBehaviour
    {

        static int blockIdx = 0;
        static float[] blockRandomPositions = new float[] { 0, 120, 40, 270, 90, 10, 160, 300 };

        [SerializeField] float _disappearDuration = 2;
        [SerializeField] float _rotationSpeed = 720;

        NodeStateVisualsDriver _valiNode;

        [SerializeField] Transform _blockPosition;
        [SerializeField] LineRenderer _line;
        [SerializeField] Transform _lineEndVisuals;
        [SerializeField] int _curveResolution = 8;
        [SerializeField] Vector2 _curveStartEnd = new Vector2(0, 1);
        [SerializeField] Animator _curveAnimator;
        [SerializeField] Animator _blockAnimator;
        [SerializeField] float _animationDuration = 3;


        [SerializeField] GameObject _validationEffectPrefab;

        Vector2 _prevCurveStartEnd = Vector2.zero;

        /// <summary> Attach the validator node visually. Null to remove attachment. </summary>
        public void AttachNode(GameObject node)
        {
            _prevCurveStartEnd = _curveStartEnd;
            _valiNode = node != null ? node.GetComponent<NodeStateVisualsDriver>() : null;
            _curveAnimator.Play("BlockTetherAppear");
        }



        /// <summary> Spawns the needed visuals for showing the block has been validated </summary>
        public IEnumerator Validate()
        {
            var op = Pool.Instance.GetSpawnFromPrefab(_validationEffectPrefab);
            op.transform.position = _blockPosition.position;
            op.SetActive(true);
            _curveAnimator.SetTrigger("Validate");
            _blockAnimator.SetTrigger("Trigger");

            yield return new WaitForSeconds(_animationDuration);

            _blockAnimator.gameObject.SetActive(false);

            yield break;
        }

        void OnDisable()
        {
            _curveAnimator.ResetTrigger("Validate");
            _blockAnimator.ResetTrigger("Trigger");
            _line.enabled = false;
            _line.positionCount = 0;
            _lineEndVisuals.gameObject.SetActive(false);
            _valiNode = null;
        }

        void Start()
        {

        }

        void Update()
        {
            _line.enabled = _valiNode != null;
            _lineEndVisuals.gameObject.SetActive(_valiNode != null);
            if (_valiNode == null) return;
            _line.useWorldSpace = true;
            if (_prevCurveStartEnd != _curveStartEnd)
            {
                DrawBezier(_curveResolution, _curveStartEnd.x, _curveStartEnd.y);
                _prevCurveStartEnd = _curveStartEnd;
            }

        }

        #region IWorldBehaviour
        public void FixedUpdateBehaviour(float dt) { }

        public void InitBehaviour()
        {
            transform.localRotation = Quaternion.AngleAxis(blockRandomPositions[blockIdx], Vector3.up);
            blockIdx++;
            if (blockIdx >= blockRandomPositions.Length) blockIdx = 0;

            _blockAnimator.gameObject.SetActive(true);
        }



        public void UpdateBehaviour(float dt) { }

        public void UpdateLOD(int lodLevel) { }

        public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter) { }
        #endregion


        Vector3[] _positions;
        void DrawBezier(int pointCount, float start, float end)
        {

            pointCount = Mathf.Min(30, pointCount);
            _positions = new Vector3[pointCount];

            var p0 = _valiNode.transform.position;
            var p3 = _blockPosition.position;

            var p1 = p0;
            var p2 = p3;
            var midY = (p0.y + p3.y) / 2f;
            p1.y = midY;
            p2.y = midY;

            _lineEndVisuals.position = p0;

            for (int i = 0; i < pointCount; i++)
            {
                float t = Mathf.Lerp(start, end, i / ((float)pointCount - 1));
                _positions[i] = GetPoint(p0, p1, p2, p3, t);
            }


            _line.positionCount = pointCount;
            _line.SetPositions(_positions);
        }

        static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }
    }
}


