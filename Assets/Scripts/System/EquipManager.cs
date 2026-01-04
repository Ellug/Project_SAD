using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipManager : SingletonePattern<EquipManager>
{
    [Header("Default")]
    [SerializeField] private WeaponBase _defaultWeapon;

    public WeaponBase Weapon { get; private set; }                 // 선택된 무기 프리팹
    public WeaponBase CurrentWeaponInstance { get; private set; }  // 실제 장착된 무기 인스턴스

    public event Action<WeaponBase> OnWeaponEquipped;

    private GameManager _gm;
    private int[] _perkSelections; // 전체 퍽 선택 상태 저장
    private bool _suppressPerksChanged;

    // Perks -> PlayerMods 브릿지용
    private PlayerModel _boundPlayer;
    private PerksTree _boundTree;

    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.sceneLoaded += OnSceneLoad;
        
        _gm = GameManager.Instance;
        if (_gm != null)
            _gm.OnUnlockStageChanged += OnUnlockStageChanged;
    }

    void OnDisable()
    {        
        SceneManager.sceneLoaded -= OnSceneLoad;

        if (_gm != null)
            _gm.OnUnlockStageChanged -= OnUnlockStageChanged;
    }

    void Start()
    {
        EnsureDefaultWeapon();
        TryEquipOnCurrentScene();
    }

    void OnDestroy()
    {
        UnbindPerksBridge();
    }


    private void EnsureDefaultWeapon()
    {
        if (Weapon == null && _defaultWeapon != null)
            Weapon = _defaultWeapon;
    }

    private void TryEquipOnCurrentScene()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name == "Lobby")
            EquipPlayerWeapon();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Stage") || scene.name == "Lobby")
            EquipPlayerWeapon();
    }

    public void SetPlayerWeapon(WeaponBase weapon)
    {
        if (Weapon != null && Weapon.GetWeaponId() == weapon.GetWeaponId()) return;

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

        // 같은 무기면 "재생성" 하지 말고, 현재 인스턴스 기준으로 UI/브릿지 재동기화만
        if (CurrentWeaponInstance != null && CurrentWeaponInstance.GetWeaponId() == Weapon.GetWeaponId())
        {
            ApplySelectionsToTree(CurrentWeaponInstance.PerksTree, notify: true);

            // 트리/브릿지/플레이어 적용만 보정
            BindPerksBridge(player, CurrentWeaponInstance.PerksTree);
            ApplyPerksTo(player, CurrentWeaponInstance);

            OnWeaponEquipped?.Invoke(CurrentWeaponInstance);
            return;
        }

        // 로비/스테이지에서 FirePoint 밑에 무기 중복 생성 방지
        for (int i = firePoint.transform.childCount - 1; i >= 0; i--)
            Destroy(firePoint.transform.GetChild(i).gameObject);

        var weaponObj = Instantiate(Weapon.gameObject, firePoint.transform);
        var weaponInstance = weaponObj.GetComponent<WeaponBase>();

        CurrentWeaponInstance = weaponInstance;

        // 1) Player에 장착 (이 시점에 PlayerStatsContext.SetWeapon()이 호출되어 WeaponMods는 자동 처리됨)
        player.SetWeapon(weaponInstance);

        // 2) Perks 복원
        ApplySelectionsToTree(weaponInstance.PerksTree, notify: true);

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
        if (_suppressPerksChanged) return;

        // 1) 트리에서 읽고 저장
        _perkSelections = _boundTree.ExportSelections();

        // 2) 특전에 적용 (잠긴 스테이지 -1 강제 / 열린 스테이지 -1이면 0)
        int[] applied = ApplyUnlockPerks(_perkSelections, _boundTree.StageCount, _gm.UnlockStage);

        if (!AreEqual(_perkSelections, applied))
        {
            _perkSelections = applied;

            // 트리에 반영하되, 이벤트 루프 방지
            _suppressPerksChanged = true;
            _boundTree.ImportSelections(_perkSelections, notify: false);
            _suppressPerksChanged = false;
        }

        if (_boundPlayer == null) return;

        var mods = _boundTree.GetActiveMods();
        _boundPlayer.RebuildRuntimeStats(mods);
    }

    // 퍽 선택 동일한지 비교용 메서드
    private bool AreEqual(int[] a, int[] b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        if (a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return false;

        return true;
    }

    // i < unlockedCount: -1이면 0으로 채움 (기존 0/1 유지)
    // i >= unlockedCount: 무조건 -1
    private int[] ApplyUnlockPerks(int[] src, int stageCount, int unlockStage)
    {
        var dst = new int[stageCount];
        for (int i = 0; i < stageCount; i++) dst[i] = -1;

        if (src != null)
        {
            int copy = Mathf.Min(src.Length, stageCount);
            for (int i = 0; i < copy; i++)
            {
                int v = src[i];
                dst[i] = (v == -1 || v == 0 || v == 1) ? v : -1;
            }
        }

        int unlockedCount = Mathf.Clamp(unlockStage, 0, stageCount);

        for (int i = 0; i < unlockedCount; i++)
            if (dst[i] == -1) dst[i] = 0;

        for (int i = unlockedCount; i < stageCount; i++)
            dst[i] = -1;

        return dst;
    }

    private void ApplySelectionsToTree(PerksTree tree, bool notify)
    {
        if (tree == null) return;

        int stageCount = tree.StageCount;

        int unlockStage = _gm.UnlockStage;
        int[] normalized = ApplyUnlockPerks(_perkSelections, stageCount, unlockStage);

        _perkSelections = normalized;

        if (notify)
        {
            // notify:true면 OnChanged -> OnBoundPerksChanged가 돌아야 함 (스탯/저장 갱신)
            tree.ImportSelections(_perkSelections, notify: true);
        }
        else
        {
            // notify:false일 때만 루프 방지
            _suppressPerksChanged = true;
            tree.ImportSelections(_perkSelections, notify: false);
            _suppressPerksChanged = false;
        }
    }

    private void OnUnlockStageChanged(int unlockStage)
    {
        if (_boundTree == null) return;

        ApplySelectionsToTree(_boundTree, notify: true);
    }
}
