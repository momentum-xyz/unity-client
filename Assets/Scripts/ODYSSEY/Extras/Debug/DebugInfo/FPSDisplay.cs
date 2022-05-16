using UnityEngine;
using UnityEngine.UI;

namespace Odyssey
{
    [RequireComponent(typeof(FPSMonitor))]
    public class FPSDisplay : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        float UpdateRatePerSec = 1f;

        [Header("Refs")]
        [SerializeField] Text FpsText = null;
        [SerializeField] Text MsText = null;

        [SerializeField] Text AvgFpsText = null;
        [SerializeField] Text MinFpsText = null;
        [SerializeField] Text MaxFpsText = null;

        void Awake()
        {
            _fpsMonitor = GetComponent<FPSMonitor>();
        }

        void Start()
        {
            _time = 0;
            _frameCount = 0;
        }

        void Update()
        {
            _time += Time.unscaledDeltaTime;
            _frameCount++;

            if (_time < UpdateRatePerSec) return;

            var fps = _frameCount / _time;
            FpsText.text = Mathf.RoundToInt(fps).ToString();
            MsText.text = (_time / _frameCount * 1000f).ToString("0.0");
            MinFpsText.text = Mathf.RoundToInt(_fpsMonitor.MinFps).ToString();
            SetFpsRelatedTextColor(MinFpsText, _fpsMonitor.MinFps);
            MaxFpsText.text = Mathf.RoundToInt(_fpsMonitor.MaxFps).ToString();
            SetFpsRelatedTextColor(MaxFpsText, _fpsMonitor.MaxFps);
            AvgFpsText.text = Mathf.RoundToInt(_fpsMonitor.AverageFps).ToString();
            SetFpsRelatedTextColor(AvgFpsText, _fpsMonitor.AverageFps);

            _time = 0f;
            _frameCount = 0;

        }

        private void SetFpsRelatedTextColor(Text text, float fps)
        {
            if (fps >= 30)
            {
                text.color = _goodColor;
            }
            else if (fps > 15)
            {
                text.color = _cautionColor;
            }
            else
            {
                text.color = _criticalColor;
            }
        }

        FPSMonitor _fpsMonitor;
        float _time;
        int _frameCount;

        readonly Color32 _goodColor = new Color32(118, 212, 58, 255);
        readonly Color32 _cautionColor = new Color32(243, 232, 0, 255);
        readonly Color32 _criticalColor = new Color32(220, 41, 30, 255);
    }
}