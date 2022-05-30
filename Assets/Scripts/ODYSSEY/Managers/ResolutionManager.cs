using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public interface IResolutionManager
{
    bool Enabled { get; set; }
}

public class ResolutionManager : MonoBehaviour, IResolutionManager
{
    [SerializeField]
    int TargetFramerate = 30;

    [SerializeField]
    float MinResolutionScale = 0.5f;

    [SerializeField]
    float MaxResolutionScale = 1.0f;

    [SerializeField]
    float ScaleChangeCooldown = 10f;

    [SerializeField]
    float MinimumDifferenceForRenderScaleChange = 0.1f;

    [SerializeField]
    int FrameHistorySize = 10;

    [SerializeField]
    AnimationCurve WeightCurve;

    public bool Enabled { get; set; }

    void OnDisable()
    {
        if (_currentPipeline)
            _currentPipeline.renderScale = 1f;
    }

    void Start()
    {
        _currentPipeline = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        _frameBuffer = new float[FrameHistorySize];
        _frameIndex = 0;
        _frameCounter = 0;
        _targetFrameMs = 1 / (float)TargetFramerate;
        _timeLastScaleChange = Time.time;

        _normalizedWeights = new float[FrameHistorySize];
        float wieghtSum = 0;
        for (int i = 0; i < FrameHistorySize; i++)
        {
            float time = (float)i / (float)FrameHistorySize;
            _normalizedWeights[i] = WeightCurve.Evaluate(time);
            wieghtSum += _normalizedWeights[i];
        }
        for (int i = 0; i < FrameHistorySize; i++)
        {
            _normalizedWeights[i] /= wieghtSum;
        }
    }

    void Update()
    {
        if (!Enabled) return;

        _frameBuffer[_frameIndex] = Time.deltaTime;
        _frameIndex = (_frameIndex + 1) % FrameHistorySize;

        //wait until buffer is filled
        if (_frameCounter++ < FrameHistorySize) return;

        //render scale cooldown
        if (Time.time - _timeLastScaleChange < ScaleChangeCooldown) return;

        float averageFrame = SumFrames();
        float targetScale = _targetFrameMs / averageFrame;
        targetScale = Mathf.Clamp(targetScale, MinResolutionScale, MaxResolutionScale);
        
        if (Mathf.Abs(targetScale - _currentPipeline.renderScale) < MinimumDifferenceForRenderScaleChange) return;

        _currentPipeline.renderScale = targetScale;
        _timeLastScaleChange = Time.time;
        Debug.LogFormat("[ResolutionManager] Render scaled changed to {0}", targetScale);
    }

    float SumFrames()
    {
        float sum = 0;
        int index = _frameIndex;
        for (int i = 0; i < _frameBuffer.Length; i++)
        {
            if (--index < 0)
                index = _frameBuffer.Length - 1;

            sum += _frameBuffer[index] * _normalizedWeights[i];
        }
        return sum;
    }

    UniversalRenderPipelineAsset _currentPipeline;
    float[] _frameBuffer;
    float[] _normalizedWeights;
    int _frameIndex;
    float _targetFrameMs;
    int _frameCounter;
    float _timeLastScaleChange;
}
