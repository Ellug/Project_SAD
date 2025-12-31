using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundEntry<T>
{
    public T key;        // Enum 타입
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
}

[Serializable]
public struct WeaponSoundEntry
{
    public WeaponEnum weapon;
    public WeaponSoundEnum soundType;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
}

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObject/Sound")]
public class SoundData : ScriptableObject
{
    public List<WeaponSoundEntry> WeaponSounds = new List<WeaponSoundEntry>();
    public List<SoundEntry<BGMEnum>> BGMSounds = new List<SoundEntry<BGMEnum>>();
    public List<SoundEntry<UIEnum>> UISounds = new List<SoundEntry<UIEnum>>();
    public List<SoundEntry<PatternEnum>> PatternSounds = new List<SoundEntry<PatternEnum>>();
    public List<SoundEntry<BossEnum>> BossSounds = new List<SoundEntry<BossEnum>>();

    private Dictionary<(WeaponEnum, WeaponSoundEnum), (AudioClip clip, float volume)> _weaponLookup;
    private Dictionary<BGMEnum, (AudioClip clip, float volume)> _bgmLookup;
    private Dictionary<UIEnum, (AudioClip clip, float volume)> _UILookup;
    private Dictionary<PatternEnum, (AudioClip clip, float volume)> _patternLookup;
    private Dictionary<BossEnum, (AudioClip clip, float volume)> _bossLookup;


    public (AudioClip clip, float volume) GetBGM(BGMEnum key) 
        => _bgmLookup.TryGetValue(key, out var clip) ? clip : default;

    public (AudioClip clip, float volume) GetWeapon(WeaponEnum weapon, WeaponSoundEnum type) 
        => _weaponLookup.TryGetValue((weapon, type), out var clip) ? clip : default;

    public (AudioClip clip, float volume) GetUI(UIEnum key) 
        => _UILookup.TryGetValue(key, out var clip) ? clip : default;

    public (AudioClip clip, float volume) GetPattern(PatternEnum key) 
        => _patternLookup.TryGetValue(key, out var clip) ? clip : default;

    public (AudioClip clip, float volume) GetBoss(BossEnum key) 
        => _bossLookup.TryGetValue(key, out var clip) ? clip : default;



    public void Initialize()
    {
        if (_weaponLookup != null)
            return;

        _bgmLookup = new Dictionary<BGMEnum, (AudioClip clip, float volume)>();
        foreach (var entry in BGMSounds)
            _bgmLookup[entry.key] = (entry.clip, entry.volume);

        _weaponLookup = new Dictionary<(WeaponEnum,WeaponSoundEnum), (AudioClip clip, float volume)>();
        foreach (var entry in WeaponSounds)
            _weaponLookup[(entry.weapon, entry.soundType)] = (entry.clip, entry.volume);

        _UILookup = new Dictionary<UIEnum, (AudioClip clip, float volume)>();
        foreach (var entry in UISounds)
            _UILookup[entry.key] = (entry.clip, entry.volume);

        _patternLookup = new Dictionary<PatternEnum, (AudioClip clip, float volume)>();
        foreach (var entry in PatternSounds)
            _patternLookup[entry.key] = (entry.clip, entry.volume);

        _bossLookup = new Dictionary<BossEnum, (AudioClip clip, float volume)>();
        foreach (var entry in BossSounds)
            _bossLookup[entry.key] = (entry.clip, entry.volume);
    }

}