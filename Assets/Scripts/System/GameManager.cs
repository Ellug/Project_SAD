using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Result
}

public class GameManager : SingletonePattern<GameManager>
{
    public GameState CurrentState { get; private set; } = GameState.Playing;
    public bool IsPlayerWin { get; private set; }

    public WeaponBase Weapon { get; private set; } // 프리팹 참조
    public WeaponBase CurrentWeaponInstance { get; private set; } // 실제 현재 장착 무기
    public event Action<WeaponBase> OnWeaponEquipped;

    private int[] _perkSelections; // 딕셔너리 같은거 써서 전체 퍽 상태 저장하는 방식으로 변경 할 수도...
    // 아니면 아예 웨펀, 퍽스 관련을 따로 좀 빼는 것도 방법. 게임 매니져가 점점 지저분해짐.

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused  => CurrentState == GameState.Paused;
    public bool IsResult  => CurrentState == GameState.Result;

    public event Action<GameState> OnGameStateChanged;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void PlayerWin()
    {
        IsPlayerWin = true;
        SetState(GameState.Result);
    }

    public void PlayerLose()
    {
        IsPlayerWin = false;
        SetState(GameState.Result);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;

        OnGameStateChanged?.Invoke(newState);
    }

    public void TogglePause()
    {
        switch (CurrentState)
        {
            case GameState.Result:
                break;
            case GameState.Playing:
                SetState(GameState.Paused);
                break;
            case GameState.Paused:
                SetState(GameState.Playing);
                break;
        }
    }

    public void ResumeGame()
    {
        SetState(GameState.Playing);
    }

    public void GoToLobby()
    {
        SetState(GameState.Playing);
        SceneManager.LoadScene("Lobby");
    }

    public void ReloadCurrentScene()
    {
        SetState(GameState.Playing);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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


    public void GameExit()
    {
        Application.Quit();
    }
}
