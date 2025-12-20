using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipManager : SingletonePattern<EquipManager>
{
    public WeaponBase Weapon { get; private set; }                 // 선택된 무기 프리팹
    public WeaponBase CurrentWeaponInstance { get; private set; }  // 실제 장착된 무기 인스턴스

    public event Action<WeaponBase> OnWeaponEquipped;

    private int[] _perkSelections; // 전체 퍽 선택 상태 저장

    // Perks -> PlayerMods 브릿지용
    private PlayerModel _boundPlayer;
    private PerksTree _boundTree;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        UnbindPerksBridge();
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

        // 1) Player에 장착 (이 시점에 PlayerStatsContext.SetWeapon()이 호출되어 WeaponMods는 자동 처리됨)
        player.SetWeapon(weaponInstance);

        // 2) Perks 복원
        RestorePerksTo(weaponInstance.PerksTree);

        // 3) 무기 퍽이 플레이어에도 영향을 주기에 바인딩
        BindPerksBridge(player, weaponInstance.PerksTree);

        // 4) PlayerMods 갱신
        ApplyPerksTo(player, weaponInstance);

        // UI가 복원된 트리 참조
        OnWeaponEquipped?.Invoke(weaponInstance);
    }

    private void ApplyPerksTo(PlayerModel player, WeaponBase weapon)
    {
        if (player == null || weapon == null) return;
        if (weapon.PerksTree == null) return;

        var mods = weapon.PerksTree.GetActiveMods();

        // PlayerMods만 갱신 (WeaponMods는 PlayerStatsContext가 PerksTree 구독으로 자동 갱신)
        player.RebuildRuntimeStats(mods);
    }

    // Perks Bridge
    private void BindPerksBridge(PlayerModel player, PerksTree tree)
    {
        UnbindPerksBridge();

        _boundPlayer = player;
        _boundTree = tree;

        if (_boundTree != null)
            _boundTree.OnChanged += OnBoundPerksChanged;

        // 바인딩 직후 현재 상태를 즉시 저장/반영
        OnBoundPerksChanged();
    }

    private void UnbindPerksBridge()
    {
        if (_boundTree != null)
            _boundTree.OnChanged -= OnBoundPerksChanged;

        _boundTree = null;
        _boundPlayer = null;
    }

    private void OnBoundPerksChanged()
    {
        if (_boundTree == null) return;

        // 1) 저장을 먼저 갱신
        SavePerksFrom(_boundTree);

        if (_boundPlayer == null) return;

        // 2) 무기 퍽 변경 시, 플레이어 스탯도 같이 갱신
        var mods = _boundTree.GetActiveMods();
        _boundPlayer.RebuildRuntimeStats(mods);
    }

    // Save / Restore
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
