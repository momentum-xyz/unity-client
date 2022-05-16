using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class MorphManager : MonoBehaviour
    {
        //
        // STATIC
        //
        public static MorphManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var op = new GameObject();
                    op.name = "[WavesUniverseManager]";
                    _instance = op.AddComponent<MorphManager>();
                }
                return _instance;
            }
        }
        static MorphManager _instance;


        static HashSet<MorphElement> _elements = new HashSet<MorphElement>();
        public static void Register(MorphElement elm)
        {
            var a = Instance; // forces spawn if necessary
            _elements.Add(elm);
        }
        public static void Deregister(MorphElement elm)
        {
            _elements.Remove(elm);
        }


        //
        // INSTANCE
        //

        [SerializeField] float _morphDistance = 100;

        Transform _cam;

        void Awake()
        {
            if (_instance != this && _instance != null) Destroy(_instance.gameObject);
            _instance = this;
        }

        void OnDisable()
        {
            _isProbing = false;
        }

        void Update()
        {
            UpdateState();
        }

        bool _isProbing;
        IEnumerator ProbeForCam()
        {
            _isProbing = true;
            while (_cam == null)
            {
                _cam = Camera.main?.transform;
                yield return new WaitForSeconds(1f);
            }
            _isProbing = false;
        }



        void UpdateState()
        {
            if (!_cam)
            {
                if (!_isProbing) StartCoroutine(ProbeForCam());
                return;
            }

            var phase = Mathf.InverseLerp(-_morphDistance, _morphDistance, _cam.position.x);
            foreach (var elm in _elements)
                elm.UpdatePhase(phase);
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.left * _morphDistance, Vector3.right * _morphDistance);
            Gizmos.DrawCube(Vector3.left * _morphDistance, Vector3.one * _morphDistance * 0.12f);
            Gizmos.DrawCube(Vector3.right * _morphDistance, Vector3.one * _morphDistance * 0.12f);
        }
    }
}