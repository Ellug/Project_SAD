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

    private readonly Dictionary<int, StatMod[]> _dynamicModsByOwner = new();

    // for Debuff
    private float _activeDebuffRemain;

    // 디버프는 동적 모드 딕셔너리에 예약 키로 합산
    private const int DebuffOwnerId = 1001;

    public PlayerModel Model => _playerModel;

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

        // 대기실 장착이라면 잔여 런타임 효과는 전부 초기화
        _activeBuffs.Clear();
        _dynamicModsByOwner.Clear();
        _activeDebuffRemain = 0f;

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
    public void Rebuild()
    {
        if (_playerModel == null)
            return;

        IEnumerable<StatMod> buffMods = EnumerateActiveBuffMods();
        IEnumerable<StatMod> dynMods = EnumerateDynamicMods();

        // Player = PlayerMods + BuffMods + DynMods
        var playerAll = ConcatMods(ConcatMods(_playerMods, buffMods), dynMods);

        PlayerRuntimeStats playerStats = _playerModel.CaptureBaseStatsSnapshot();
        PerkCalculator.ApplyToPlayer(ref playerStats, playerAll);

        // Weapon = WeaponMods + BuffMods + DynMods
        WeaponRuntimeStats weaponStats = default;

        if (_weapon != null && _weapon.WeaponData != null)
        {
            var weaponAll = ConcatMods(ConcatMods(_weaponMods, buffMods), dynMods);

            weaponStats = WeaponRuntimeStats.FromData(_weapon.WeaponData);
            PerkCalculator.ApplyToWeapon(ref weaponStats, weaponAll);
        }

        Current.Player = playerStats;
        Current.Weapon = weaponStats;
        Current.UpdateDerived();

        OnChanged?.Invoke();
    }

    public void Trigger(PerkTrigger trigger)
    {
        if (_triggeredBuffs == null) return;

        bool changed = false;
        float healAmount = 0f;

        foreach (var buff in _triggeredBuffs)
        {
            if (buff == null) continue;
            if (buff.trigger != trigger) continue;

            if (buff.healPerTrigger > 0f)
                healAmount += buff.healPerTrigger;

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

        if (healAmount > 0f && _playerModel != null)
            _playerModel.TakeHeal(healAmount);

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

    public void SetDynamicMods(int ownerId, StatMod[] mods)
    {
        if (ownerId == 0) return;

        bool changed;

        if (mods == null || mods.Length == 0)
        {
            changed = _dynamicModsByOwner.Remove(ownerId);
        }
        else
        {
            _dynamicModsByOwner[ownerId] = mods;
            changed = true;
        }

        if (changed)
            Rebuild();
    }

    private IEnumerable<StatMod> EnumerateDynamicMods()
    {
        foreach (var kv in _dynamicModsByOwner)
        {
            var arr = kv.Value;
            if (arr == null) continue;

            for (int i = 0; i < arr.Length; i++)
                yield return arr[i];
        }
    }

    // Debuff
    public void ApplyDebuff(StatMod[] mods, float duration)
    {
        if (mods == null || mods.Length == 0) return;

        duration = Mathf.Max(0.01f, duration);

        // 디버프 지속시간 갱신
        _activeDebuffRemain = Mathf.Max(_activeDebuffRemain, duration);

        // 디버프 스탯은 동적 모드 딕셔너리에 합산
        SetDynamicMods(DebuffOwnerId, mods); // 내부에서 Rebuild 호출
    }

    public void TickDynamicDebuffs(float dt)
    {
        if (_activeDebuffRemain <= 0f) return;

        _activeDebuffRemain -= dt;

        if (_activeDebuffRemain <= 0f)
        {
            _activeDebuffRemain = 0f;

            // 디버프 키 제거 (내부에서 Rebuild 호출)
            SetDynamicMods(DebuffOwnerId, null);
        }
    }
}
