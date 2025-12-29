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

    private void PlayWeaponSound(WeaponRuntimeStats stats, WeaponSoundEnum type, AudioSource audioSource)
    {
        var clip = _soundData.GetWeapon(stats.WeaponEnum, type);
        if (clip == null) return;

        audioSource.PlayOneShot(clip, 0.5f);
    }

    public void PlayBGM(BGMEnum bgmType)
    {
        var clip = _soundData.GetBGM(bgmType);
        if (clip == null) return;

        if (_bgmSource.clip == clip && _bgmSource.isPlaying)
            return;

        _bgmSource.clip = clip;
        _bgmSource.Play();
    }
}