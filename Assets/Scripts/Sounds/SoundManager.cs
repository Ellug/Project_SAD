using UnityEngine;

public class SoundManager : SingletonePattern<SoundManager>
{
    [SerializeField] private SoundData _soundData;

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _UISource;

    protected override void Awake()
    {
        base.Awake();
        _soundData.Initialize();
    }

    public void BindWeapon(WeaponBase weapon, AudioSource weaponAudio)
    {
        weapon.OnFire += (stats) =>
            PlayWeaponSound(stats, WeaponSoundEnum.NormalAttack, weaponAudio);

        weapon.OnSPFire += (stats) =>
            PlayWeaponSound(stats, WeaponSoundEnum.SpecialAttack, weaponAudio);

        weapon.OnReload += (stats) =>
            PlayWeaponSound(stats, WeaponSoundEnum.Reload, weaponAudio);
    }
    public void BindPattern(PatternBase pattern, AudioSource patternAudio)
    {
        pattern.OnPatternSound += (patternType) =>
            PlayPatternSound(patternType, patternAudio);
    }

    private void PlayPatternSound(PatternEnum patternType, AudioSource audioSource)
    {
        var data = _soundData.GetPattern(patternType);
        if (data.clip == null) return;

        audioSource.PlayOneShot(data.clip, data.volume);
    }

    private void PlayWeaponSound(WeaponRuntimeStats stats, WeaponSoundEnum type, AudioSource audioSource)
    {
        var data = _soundData.GetWeapon(stats.WeaponEnum, type);
        if (data.clip == null) return;

        audioSource.PlayOneShot(data.clip, data.volume);
    }

    public void PlayBGM(BGMEnum bgmType)
    {
        var data = _soundData.GetBGM(bgmType);
        if (data.clip == null) return;

        if (_bgmSource.clip == data.clip && _bgmSource.isPlaying)
            return;

        _bgmSource.clip = data.clip;
        _bgmSource.volume = data.volume;
        _bgmSource.Play();
    }
}