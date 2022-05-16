using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Odyssey
{
    public class FPSMonitor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        int SamplesBufferSize = 150;

        [SerializeField]
        int TimeToResetMinMaxFps = 10;

        public float CurrentFps { get; private set; }
        public float AverageFps { get; private set; }
        public float MinFps { get; private set; }
        public float MaxFps { get; private set; }

        void Awake()
        {
            _samplesCapacity = Mathf.NextPowerOfTwo(SamplesBufferSize);

            _samples = new float[_samplesCapacity];

            _indexMask = _samplesCapacity - 1;

            _timeToResetMinFpsPassed = 0;
            _timeToResetMaxFpsPassed = 0;
            _samplesCount = 0;
        }

        void Update()
        {
            var unscaledDeltaTime = Time.unscaledDeltaTime;

            _timeToResetMinFpsPassed += unscaledDeltaTime;
            _timeToResetMaxFpsPassed += unscaledDeltaTime;

            CurrentFps = 1 / unscaledDeltaTime;

            _samples[ToBufferIndex(_samplesCount)] = CurrentFps;

            if (_samplesCount < _samplesCapacity)
                _samplesCount++;

            for (int i = 0; i < _samplesCount; i++)
            {
                AverageFps += _samples[i];
            }
            AverageFps /= _samplesCount;

            if (_timeToResetMaxFpsPassed > TimeToResetMinMaxFps)
            {
                MinFps = 0;
                _timeToResetMaxFpsPassed = 0;
            }

            if (_timeToResetMinFpsPassed > TimeToResetMinMaxFps)
            {
                MaxFps = 0;
                _timeToResetMinFpsPassed = 0;
            }

            if (CurrentFps < MinFps || MinFps <= 0)
            {
                MinFps = CurrentFps;
                _timeToResetMinFpsPassed = 0;
            }
            if (CurrentFps > MaxFps || MaxFps <= 0)
            {
                MaxFps = CurrentFps;
                _timeToResetMaxFpsPassed = 0;
            }
        }

#if NET_4_6 || NET_STANDARD_2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private int ToBufferIndex(int index)
        {
            return index & _indexMask;
        }

        float[] _samples;
        int _samplesCapacity;
        int _samplesCount;
        int _indexMask;


        float _timeToResetMinFpsPassed;
        float _timeToResetMaxFpsPassed;
    }
}