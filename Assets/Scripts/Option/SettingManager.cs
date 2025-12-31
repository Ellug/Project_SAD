using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingManager : SingletonePattern<SettingManager>
{
    [SerializeField] private SettingData _defaultData;
    [SerializeField] private AudioMixer _audioMixer;

    public SettingData Data { get; private set; }

    public event Action OnSettingApplied;
    public event Action OnSettingChanged;

    // UI/적용 로직이 함께 쓰는 단일 해상도 목록
    private readonly List<Resolution> _availableResolutions = new();
    public IReadOnlyList<Resolution> AvailableResolutions => _availableResolutions;

    protected override void Awake()
    {
        base.Awake();

        if (Data != null)
            return;

        Data = Instantiate(_defaultData);

        BuildAvailableResolutions_16x9();
        Data.resolutionIndex = FindBestIndexForCurrentScreen();

        ApplyAll();
    }

    public void ApplyAll()
    {
        ApplyAudio();
        ApplyGraphics();
        OnSettingApplied?.Invoke();
    }

    public void SetMasterVolume(float value)
    {
        Data.masterVolume = value;
        ApplyAudio();
        OnSettingChanged?.Invoke();
    }

    public void SetEffectVolume(float value)
    {
        Data.effectVolume = value;
        ApplyAudio();
        OnSettingChanged?.Invoke();
    }

    public void SetBGMVolume(float value)
    {
        Data.BGMVolume = value;
        ApplyAudio();
        OnSettingChanged?.Invoke();
    }

    public void SetMute(bool mute)
    {
        if (mute == Data.isMute)
            return;

        Data.isMute = mute;

        if (mute)
        {
            Data.prevMasterVolume = Data.masterVolume;
            Data.masterVolume = 0f;
        }
        else
        {
            Data.masterVolume = Data.prevMasterVolume;
        }

        ApplyAudio();
        OnSettingChanged?.Invoke();
    }

    private void ApplyAudio()
    {
        float master = Data.masterVolume;
        float effect = master * Data.effectVolume;
        float bgm = master * Data.BGMVolume;

        _audioMixer.SetFloat("MasterVolume", ToDecibel(master));
        _audioMixer.SetFloat("EffectVolume", ToDecibel(effect));
        _audioMixer.SetFloat("BGMVolume", ToDecibel(bgm));
    }

    public void SetResolution(int index)
    {
        // 이제 index는 "AvailableResolutions" 기준 인덱스
        Data.resolutionIndex = index;
        ApplyGraphics();
        OnSettingChanged?.Invoke();
    }

    public void SetScreenMode(FullScreenMode mode)
    {
        Data.screenMode = mode;
        ApplyGraphics();
        OnSettingChanged?.Invoke();
    }

    private void ApplyGraphics()
    {
        if (_availableResolutions.Count == 0)
        {
            // 혹시라도 비었으면 다시 빌드
            BuildAvailableResolutions_16x9();
            if (_availableResolutions.Count == 0)
                return;
        }

        int idx = Mathf.Clamp(Data.resolutionIndex, 0, _availableResolutions.Count - 1);
        Data.resolutionIndex = idx;

        Resolution r = _availableResolutions[idx];

#if UNITY_2022_2_OR_NEWER
        Screen.SetResolution(r.width, r.height, Data.screenMode, r.refreshRateRatio);
#else
        int hz = Mathf.RoundToInt((float)r.refreshRateRatio.value);
        Screen.SetResolution(r.width, r.height, Data.screenMode, hz);
#endif
    }

    private float ToDecibel(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        return Mathf.Log10(value) * 20f;
    }

    // Resolution List Build (16:9)
    private void BuildAvailableResolutions_16x9()
    {
        _availableResolutions.Clear();

        Resolution[] all = Screen.resolutions;

        // 모니터 최대 해상도 제한
        int maxW = Screen.currentResolution.width;
        int maxH = Screen.currentResolution.height;

        // (w,h)별 최고 Hz만 유지
        Dictionary<(int w, int h), Resolution> bestBySize = new();

        foreach (var r in all)
        {
            if (r.width > maxW || r.height > maxH)
                continue;

            if (!IsAspect16x9(r.width, r.height))
                continue;

            var key = (r.width, r.height);

            if (!bestBySize.TryGetValue(key, out var prev) ||
                r.refreshRateRatio.value > prev.refreshRateRatio.value)
            {
                bestBySize[key] = r;
            }
        }

        var list = new List<Resolution>(bestBySize.Values);

        // 작은 해상도 -> 큰 해상도 순 정렬
        list.Sort((a, b) =>
        {
            int c = a.width.CompareTo(b.width);
            if (c != 0) return c;
            c = a.height.CompareTo(b.height);
            if (c != 0) return c;
            return b.refreshRateRatio.value.CompareTo(a.refreshRateRatio.value);
        });

        _availableResolutions.AddRange(list);
    }

    private static bool IsAspect16x9(int w, int h)
    {
        // 1366x768 같은 "거의 16:9"도 포함되도록 허용오차 사용
        float ratio = w / (float)h;
        return Mathf.Abs(ratio - (16f / 9f)) < 0.02f;
    }

    private int FindBestIndexForCurrentScreen()
    {
        // Screen.currentResolution 대신 실제 게임 화면 기준 사용
        int curW = Screen.width;
        int curH = Screen.height;

        // 완전 일치 우선
        for (int i = 0; i < _availableResolutions.Count; i++)
        {
            if (_availableResolutions[i].width == curW &&
                _availableResolutions[i].height == curH)
                return i;
        }

        // 없으면 가장 가까운 해상도
        int best = 0;
        int bestScore = int.MaxValue;

        for (int i = 0; i < _availableResolutions.Count; i++)
        {
            int dw = Mathf.Abs(_availableResolutions[i].width - curW);
            int dh = Mathf.Abs(_availableResolutions[i].height - curH);
            int score = dw + dh;

            if (score < bestScore)
            {
                bestScore = score;
                best = i;
            }
        }

        return best;
    }
}