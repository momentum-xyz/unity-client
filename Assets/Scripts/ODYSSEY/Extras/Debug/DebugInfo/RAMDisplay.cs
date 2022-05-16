using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Odyssey
{

    public class RAMDisplay : MonoBehaviour, IInformationCollector
    {
        [Header("Settings")]
        [SerializeField]
        float UpdateRatePerSec = 1f;

        [Header("Refs")]
        [SerializeField]
        Text AllocatedSystemMemorySizeText = null;

        [SerializeField]
        Text ReservedSystemMemorySizeText = null;

        [SerializeField]
        Text MonoSystemMemorySizeText = null;


        void Start()
        {
            AllocatedSystemMemorySizeText.color = new Color32(255, 190, 60, 255);
            ReservedSystemMemorySizeText.color = new Color32(205, 84, 229, 255);
            MonoSystemMemorySizeText.color = new Color(0.3f, 0.65f, 1f, 1);
        }

        void Update()
        {
            _time += Time.unscaledDeltaTime;

            if (_time > UpdateRatePerSec)
            {
                _allocatedRam = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
                _reservedRam = Profiler.GetTotalReservedMemoryLong() / 1048576f;
                _monoRam = Profiler.GetMonoUsedSizeLong() / 1048576f;

                const string format = "0.0";
                AllocatedSystemMemorySizeText.text = _allocatedRam.ToString(format);
                ReservedSystemMemorySizeText.text = _reservedRam.ToString(format);
                MonoSystemMemorySizeText.text = _monoRam.ToString(format);

                _time = 0f;
            }
        }

        public string GetInfo()
        {
            var allSpecsInformation = new List<string>();
            allSpecsInformation.Add("Allocated RAM: " + AllocatedSystemMemorySizeText.text);
            allSpecsInformation.Add("Reserved RAM: " + ReservedSystemMemorySizeText.text);
            allSpecsInformation.Add("Mono RAM: " + MonoSystemMemorySizeText.text);

            return String.Join("\n", allSpecsInformation);
        }


        float _allocatedRam;
        float _reservedRam;
        float _monoRam;

        float _time;
    }
}
