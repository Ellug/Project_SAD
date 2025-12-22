using System;
using UnityEngine;
using UnityEngine.Audio;

public class SettingManager : SingletonePattern<SettingManager>
{
    [SerializeField] SettingData _defaultData;
    [SerializeField] private AudioMixer audioMixer;

    public SettingData Data { get; private set; }

    public event Action OnSettingApplied;
    public event Action OnSettingChanged;

    protected override void Awake()
    {
        base.Awake();
        Data = Instantiate(_defaultData);

        Data.resolutionIndex = GetCurrentResolutionIndex();

        ApplyAll();
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

    public void ApplyAll()
    {
        ApplyAudio();
        ApplyGraphics();
        OnSettingApplied?.Invoke();
    }

    private void ApplyAudio()
    {
        float master = Data.masterVolume;
        float effect = master * Data.effectVolume;
        float bgm = master * Data.BGMVolume;

        audioMixer.SetFloat("MasterVolume", ToDecibel(master));
        audioMixer.SetFloat("EffectVolume", ToDecibel(effect));
        audioMixer.SetFloat("BGMVolume", ToDecibel(bgm));
    }
    public void SetResolution(int index)
    {
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
        Resolution[] resolutions = Screen.resolutions;
        int idx = Mathf.Clamp(Data.resolutionIndex, 0, resolutions.Length - 1);

        Resolution r = resolutions[idx];
        Screen.SetResolution(r.width, r.height, Data.screenMode);
    }

    private float ToDecibel(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        return Mathf.Log10(value) * 20f;
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

    private int GetCurrentResolutionIndex()
    {
        Resolution current = Screen.currentResolution;
        Resolution[] resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == current.width &&
                resolutions[i].height == current.height)
                return i;
        }

        return resolutions.Length - 1;
    }
}
