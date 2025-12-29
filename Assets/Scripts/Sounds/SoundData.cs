using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SoundEntry<T>
{
    public T key;        // Enum 타입
    public AudioClip clip;
}

[Serializable]
public struct WeaponSoundEntry
{
    public WeaponEnum weapon;
    public WeaponSoundEnum soundType;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundData", menuName = "ScriptableObject/Sound")]
public class SoundData : ScriptableObject
{
    public List<WeaponSoundEntry> WeaponSounds = new List<WeaponSoundEntry>();
    public List<SoundEntry<BGMEnum>> BGMSounds = new List<SoundEntry<BGMEnum>>();
    public List<SoundEntry<UIEnum>> UISounds = new List<SoundEntry<UIEnum>>();
    public List<SoundEntry<PatternEnum>> PatternSounds = new List<SoundEntry<PatternEnum>>();
    public List<SoundEntry<BossEnum>> BossSounds = new List<SoundEntry<BossEnum>>();

    private Dictionary<(WeaponEnum,WeaponSoundEnum), AudioClip> _weaponLookup;
    private Dictionary<BGMEnum, AudioClip> _bgmLookup;
    private Dictionary<UIEnum, AudioClip> _UILookup;
    private Dictionary<PatternEnum, AudioClip> _patternLookup;
    private Dictionary<BossEnum, AudioClip> _bossLookup;


    public AudioClip GetBGM(BGMEnum key) 
        => _bgmLookup.TryGetValue(key, out var clip) ? clip : null;

    public AudioClip GetWeapon(WeaponEnum weapon, WeaponSoundEnum type) 
        => _weaponLookup.TryGetValue((weapon, type), out var clip) ? clip : null;

    public AudioClip GetUI(UIEnum key) 
        => _UILookup.TryGetValue(key, out var clip) ? clip : null;

    public AudioClip GetPattern(PatternEnum key) 
        => _patternLookup.TryGetValue(key, out var clip) ? clip : null;

    public AudioClip GetBoss(BossEnum key) 
        => _bossLookup.TryGetValue(key, out var clip) ? clip : null;



    public void Initialize()
    {
        if (_weaponLookup != null)
            return;

        _bgmLookup = new Dictionary<BGMEnum, AudioClip>();
        foreach (var entry in BGMSounds)
            _bgmLookup[entry.key] = entry.clip;

        _weaponLookup = new Dictionary<(WeaponEnum,WeaponSoundEnum), AudioClip>();
        foreach (var entry in WeaponSounds)
            _weaponLookup[(entry.weapon, entry.soundType)] = entry.clip;

        _UILookup = new Dictionary<UIEnum, AudioClip>();
        foreach (var entry in UISounds)
            _UILookup[entry.key] = entry.clip;

        _patternLookup = new Dictionary<PatternEnum, AudioClip>();
        foreach (var entry in PatternSounds)
            _patternLookup[entry.key] = entry.clip;

        _bossLookup = new Dictionary<BossEnum, AudioClip>();
        foreach (var entry in BossSounds)
            _bossLookup[entry.key] = entry.clip;
    }

}