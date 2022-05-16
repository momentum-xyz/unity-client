using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class MotionParticles : MonoBehaviour
    {
        [SerializeField] float _maxParticleSpeed = 1;
        [SerializeField] List<Transform> _shakers;
        [SerializeField] float _shakeSpeed = 50;
        [SerializeField] Vector3 _shakePos = 0.1f * Vector3.one;
        [SerializeField] Vector3 _shakeRot = 2 * Vector3.one;

        ThirdPersonController _controller;
        ParticleSystem _prt;
        ParticleSystem.MainModule _main;
        Vector3 _lastPos, _velo;



        void Awake()
        {
            _prt = GetComponent<ParticleSystem>();
            _main = _prt.main;
            _controller = GetComponentInParent<ThirdPersonController>();
        }

        private bool IsNaN(Quaternion q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
        }


        private bool IsNaNf(Vector3 q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z);
        }

        void Update()
        {
            float curSpeed;
            float delta = Mathf.Clamp(Time.deltaTime, 0.01f, 1f);
            if (_controller)
            {
                _velo = _controller.AbsVelo;
                curSpeed = _controller.AbsSpeed;
            }
            else
            {
                _velo = Vector3.Lerp(_velo, (transform.position - _lastPos) / delta, delta * 20);
                curSpeed = _velo.magnitude;
            }
            _lastPos = transform.position;

            if (curSpeed > 0.01f) _prt.transform.rotation = Quaternion.LookRotation(_velo);
            _main.startSpeedMultiplier = -Mathf.Max(curSpeed, _maxParticleSpeed) * 0.1f;

            if (Time.timeScale > 0)
            {
                // shake turned to NaN because of bad _controller.AbsVelo values. It is protected now.
                var shake = Mathf.Clamp01(curSpeed / _shakeSpeed);
                foreach (var shaker in _shakers)
                {
                    var newLocalPosition = Vector3.Scale(Random.insideUnitSphere, _shakePos) * shake;
                    if (newLocalPosition.IsNan() == false)
                        shaker.localPosition = newLocalPosition;
                    var newLocalEulers = Quaternion.Euler(Vector3.Scale(Random.insideUnitSphere, _shakeRot) * shake);
                    if (newLocalEulers.IsNan() == false)
                        shaker.localRotation = newLocalEulers;
                }
            }
        }
    }
}