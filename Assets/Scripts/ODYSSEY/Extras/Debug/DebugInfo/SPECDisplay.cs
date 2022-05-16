using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Odyssey
{
    public class SPECDisplay : MonoBehaviour, IInformationCollector
    {
        [Header("Settings")]
        [SerializeField]
        float UpdateRatePerSec = 1f;

        [Header("Refs")]
        [SerializeField]
        Text GraphicsDeviceVersionText = null;
        [SerializeField]
        Text ProcessorTypeText = null;
        [SerializeField]
        Text OperatingSystemText = null;
        [SerializeField]
        Text SystemMemoryText = null;
        [SerializeField]
        Text GraphicsDeviceNameText = null;
        [SerializeField]
        Text GraphicsMemorySizeText = null;
        [SerializeField]
        Text ScreenResolutionText = null;
        [SerializeField]
        Text GameWindowResolutionText = null;

        void Start()
        {
            ProcessorTypeText.text
                            = "CPU: "
                            + SystemInfo.processorType
                            + " ["
                            + SystemInfo.processorCount
                            + " cores]";

            SystemMemoryText.text
                = "RAM: "
                + SystemInfo.systemMemorySize
                + " MB";

            GraphicsDeviceVersionText.text
                = "Graphics API: "
                + SystemInfo.graphicsDeviceVersion;

            GraphicsDeviceNameText.text
                = "GPU: "
                + SystemInfo.graphicsDeviceName;

            GraphicsMemorySizeText.text
                = "VRAM: "
                + SystemInfo.graphicsMemorySize
                + "MB. Max texture size: "
                + SystemInfo.maxTextureSize
                + "px. Shader level: "
                + SystemInfo.graphicsShaderLevel;

            Resolution res = Screen.currentResolution;

            ScreenResolutionText.text
                = "Screen: "
                + res.width
                + "x"
                + res.height
                + "@"
                + res.refreshRate
                + "Hz";

            OperatingSystemText.text
                = "OS: "
                + SystemInfo.operatingSystem
                + " ["
                + SystemInfo.deviceType
                + "]";

            _sb = new StringBuilder();
        }

        void Update()
        {
            _time += Time.unscaledDeltaTime;

            if (_time > UpdateRatePerSec)
            {
                _sb.Clear();

                _sb.Append(_windowStrings[0]).Append(Screen.width.ToString())
                    .Append(_windowStrings[1]).Append(Screen.height.ToString())
                    .Append(_windowStrings[2]).Append(Screen.currentResolution.refreshRate.ToString())
                    .Append(_windowStrings[3])
                    .Append(_windowStrings[4]).Append(Screen.dpi.ToString())
                    .Append(_windowStrings[5]);

                GameWindowResolutionText.text = _sb.ToString();

                _time = 0f;
            }
        }

        public string GetInfo()
        {
            var allSpecsInformation = new List<string>();
            allSpecsInformation.Add(ProcessorTypeText.text);
            allSpecsInformation.Add(SystemMemoryText.text);
            allSpecsInformation.Add(GraphicsDeviceVersionText.text);
            allSpecsInformation.Add(GraphicsDeviceNameText.text);
            allSpecsInformation.Add(GraphicsMemorySizeText.text);
            allSpecsInformation.Add(ScreenResolutionText.text);
            allSpecsInformation.Add(OperatingSystemText.text);
            allSpecsInformation.Add("Platform: " + Application.platform.ToString());
            allSpecsInformation.Add(_sb.ToString());
            return String.Join("\n", allSpecsInformation);
        }

        float _time;
        StringBuilder _sb;
        readonly string[] _windowStrings =
        {
            "Window: ",
            "x",
            "@",
            "Hz",
            "[",
            "dpi]"
        };
    }
}
