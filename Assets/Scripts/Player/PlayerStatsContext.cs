using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsContext : MonoBehaviour
{
    [SerializeField] private PlayerModel _playerModel;

    // Sources
    private WeaponBase _weapon;
    private PerksTree _weaponPerksTree;

    private IEnumerable<StatMod> _playerMods;
    private IEnumerable<StatMod> _weaponMods;

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

        if (_weapon != null)
        {
            _weaponPerksTree = _weapon.PerksTree;

            if (_weaponPerksTree != null)
            {
                _weaponPerksTree.OnChanged += OnWeaponPerksChanged;
                _weaponMods = _weaponPerksTree.GetActiveMods();
            }
            else
            {
                _weaponMods = null;
            }
        }
        else
        {
            _weaponPerksTree = null;
            _weaponMods = null;
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
            _weaponMods = _weaponPerksTree.GetActiveMods();

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

        // Player
        PlayerRuntimeStats playerStats = _playerModel.CaptureBaseStatsSnapshot();
        PerkCalculator.ApplyToPlayer(ref playerStats, _playerMods);

        // Weapon
        WeaponRuntimeStats weaponStats = default;

        if (_weapon != null && _weapon.WeaponData != null)
        {
            weaponStats = WeaponRuntimeStats.FromData(_weapon.WeaponData);
            PerkCalculator.ApplyToWeapon(ref weaponStats, _weaponMods);
        }

        // Final
        Current.Player = playerStats;
        Current.Weapon = weaponStats;
        Current.UpdateDerived();

        OnChanged?.Invoke();
    }
}
