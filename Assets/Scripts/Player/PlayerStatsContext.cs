using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsContext : MonoBehaviour
{
    [Serializable]
    private struct ActiveBuff
    {
        public TriggeredBuff buff;
        public float remain;
    }

    [SerializeField] private PlayerModel _playerModel;

    // Sources
    private WeaponBase _weapon;
    private PerksTree _weaponPerksTree;
    private List<ActiveBuff> _activeBuffs = new();

    private IEnumerable<StatMod> _playerMods;
    private IEnumerable<StatMod> _weaponMods;
    private IEnumerable<TriggeredBuff> _triggeredBuffs;

    // Result
    public PlayerFinalStats Current { get; private set; } = new PlayerFinalStats();

    public event Action OnChanged;

    void Awake()
    {
        if (_playerModel == null)
            _playerModel = GetComponent<PlayerModel>();
    }

    private void OnDestroy()
    {
        UnbindWeaponPerks();
    }

    public void Bind(PlayerModel model)
    {
        _playerModel = model;
        Rebuild();
    }

    // Player Perks/Mods 변경 시 호출
    public void SetPlayerMods(IEnumerable<StatMod> mods)
    {
        _playerMods = mods;
        Rebuild();
    }

    // Weapon 교체 시 호출
    public void SetWeapon(WeaponBase weapon)
    {
        UnbindWeaponPerks();

        _weapon = weapon;
        _activeBuffs.Clear();

        if (_weapon != null)
        {
            _weaponPerksTree = _weapon.PerksTree;

            if (_weaponPerksTree != null)
            {
                _weaponPerksTree.OnChanged += OnWeaponPerksChanged;

                _weaponMods = _weaponPerksTree.GetActiveMods();
                _triggeredBuffs = _weaponPerksTree.GetActiveBuffs();
            }
            else
            {
                _weaponMods = null;
                _triggeredBuffs = null;
            }
        }
        else
        {
            _weaponPerksTree = null;
            _weaponMods = null;
            _triggeredBuffs = null;
        }

        Rebuild();
    }

    // Weapon -> PlayerModel 결합도 낮추고 Context를 통해서 관리
    public void NotifySpecialAttackState(bool value)
    {
        if (_playerModel == null) return;
        _playerModel.SetSpecialAttackState(value);
    }

    public void NotifyAttackSlow()
    {
        if (_playerModel == null) return;
        _playerModel.StartAttackSlow();
    }

    private void OnWeaponPerksChanged()
    {
        if (_weaponPerksTree != null)
        {
            _weaponMods = _weaponPerksTree.GetActiveMods();
            _triggeredBuffs = _weaponPerksTree.GetActiveBuffs();
        }

        _activeBuffs.Clear();

        Rebuild();
    }

    private void UnbindWeaponPerks()
    {
        if (_weaponPerksTree != null)
            _weaponPerksTree.OnChanged -= OnWeaponPerksChanged;

        _weaponPerksTree = null;
    }

    // 최종 스탯 리빌드
    // - Player Base + Player Mods -> Player Runtime
    // - Weapon Base + Weapon Mods -> Weapon Runtime
    public void Rebuild()
    {
        if (_playerModel == null)
            return;

        // 활성 버프 mods 적용
        IEnumerable<StatMod> buffMods = EnumerateActiveBuffMods();

        // Player
        PlayerRuntimeStats playerStats = _playerModel.CaptureBaseStatsSnapshot();
        PerkCalculator.ApplyToPlayer(ref playerStats, ConcatMods(_playerMods, buffMods));

        // Weapon
        WeaponRuntimeStats weaponStats = default;

        if (_weapon != null && _weapon.WeaponData != null)
        {
            weaponStats = WeaponRuntimeStats.FromData(_weapon.WeaponData);
            PerkCalculator.ApplyToWeapon(ref weaponStats, ConcatMods(_playerMods, buffMods));
        }

        // Final
        Current.Player = playerStats;
        Current.Weapon = weaponStats;
        Current.UpdateDerived();

        OnChanged?.Invoke();
    }

    public void Trigger(PerkTrigger trigger)
    {
        if (_triggeredBuffs == null) return;

        bool changed = false;

        foreach (var buff in _triggeredBuffs)
        {
            if (buff == null) continue;
            if (buff.trigger != trigger) continue;
            if (buff.mods == null || buff.mods.Length == 0) continue;

            // 이미 활성 중이면 duration으로 갱신
            int idx = FindActiveBuffIndex(buff);
            if (idx >= 0)
            {
                var b = _activeBuffs[idx];
                b.remain = Mathf.Max(b.remain, buff.duration);
                _activeBuffs[idx] = b;
            }
            else
            {
                _activeBuffs.Add(new ActiveBuff { buff = buff, remain = buff.duration });
            }

            changed = true;
        }

        if (changed)
            Rebuild();
    }

    public void TickBuffs(float dt)
    {
        if (_activeBuffs.Count == 0) return;

        bool changed = false;

        for (int i = _activeBuffs.Count - 1; i >= 0; i--)
        {
            var b = _activeBuffs[i];
            b.remain -= dt;

            if (b.remain <= 0f)
            {
                _activeBuffs.RemoveAt(i);
                changed = true;
            }
            else
            {
                _activeBuffs[i] = b;
            }
        }

        if (changed)
            Rebuild();
    }

    private int FindActiveBuffIndex(TriggeredBuff buff)
    {
        for (int i = 0; i < _activeBuffs.Count; i++)
        {
            if (ReferenceEquals(_activeBuffs[i].buff, buff))
                return i;
        }
        return -1;
    }

    private IEnumerable<StatMod> EnumerateActiveBuffMods()
    {
        for (int i = 0; i < _activeBuffs.Count; i++)
        {
            var buff = _activeBuffs[i].buff;
            if (buff?.mods == null) continue;

            for (int m = 0; m < buff.mods.Length; m++)
                yield return buff.mods[m];
        }
    }

    // 버프랑 스탯 노드를 IEnumerable에서 연속되게 이어지게 만들기 위함
    private IEnumerable<StatMod> ConcatMods(IEnumerable<StatMod> a, IEnumerable<StatMod> b)
    {
        if (a != null)
            foreach (var x in a) yield return x;

        if (b != null)
            foreach (var x in b) yield return x;
    }
}
