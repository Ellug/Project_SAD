using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipManager : SingletonePattern<EquipManager>
{
    public WeaponBase Weapon { get; private set; }                 // 선택된 무기 프리팹
    public WeaponBase CurrentWeaponInstance { get; private set; }  // 실제 장착된 무기 인스턴스

    public event Action<WeaponBase> OnWeaponEquipped;

    private int[] _perkSelections; // 전체 퍽 선택 상태 저장

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Stage") || scene.name == "Lobby")
            EquipPlayerWeapon();
    }

    public void SetPlayerWeapon(WeaponBase weapon)
    {
        // 무기 선택시 퍽 초기화. 저장 필요하면 따로 저장 시스템 도입
        if (Weapon != null && weapon != null && Weapon.GetWeaponId() != weapon.GetWeaponId())
            _perkSelections = null;

        Weapon = weapon;
    }

    public void EquipPlayerWeapon()
    {
        if (Weapon == null) return;

        var playerGo = GameObject.FindWithTag("Player");
        if (playerGo == null) return;

        var player = playerGo.GetComponent<PlayerModel>();
        if (player == null) return;

        var firePoint = GameObject.Find("FirePoint");
        if (firePoint == null) return;

        // 로비/스테이지에서 FirePoint 밑에 무기 중복 생성 방지
        for (int i = firePoint.transform.childCount - 1; i >= 0; i--)
            Destroy(firePoint.transform.GetChild(i).gameObject);

        var weaponObj = Instantiate(Weapon.gameObject, firePoint.transform);
        var weaponInstance = weaponObj.GetComponent<WeaponBase>();

        CurrentWeaponInstance = weaponInstance;

        player.SetWeapon(weaponInstance);

        RestorePerksTo(weaponInstance.PerksTree);
        ApplyPerksTo(player, weaponInstance, resetHpToMax: true);

        // UI가 반드시 복원된 트리 참조
        OnWeaponEquipped?.Invoke(weaponInstance);
    }

    private void ApplyPerksTo(PlayerModel player, WeaponBase weapon, bool resetHpToMax)
    {
        if (player == null || weapon == null) return;
        if (weapon.PerksTree == null) return;

        var mods = weapon.PerksTree.GetActiveMods();

        player.RebuildRuntimeStats(mods, resetHpToMax);
        weapon.RebuildRuntimeStats(mods);
    }

    public void SavePerksFrom(PerksTree tree)
    {
        if (tree == null) return;
        _perkSelections = tree.ExportSelections();
    }

    private void RestorePerksTo(PerksTree tree)
    {
        if (tree == null) return;
        if (_perkSelections == null) return;

        tree.ImportSelections(_perkSelections, notify: true);
    }
}
