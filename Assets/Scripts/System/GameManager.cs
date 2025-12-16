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
    public WeaponBase Weapon { get; private set; }

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused  => CurrentState == GameState.Paused;
    public bool IsResult  => CurrentState == GameState.Result;

    public event Action<GameState> OnGameStateChanged;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoad;
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
        if (scene.name.Contains("Stage"))
        {
            EquipPlayerWeapon();
        }
    }

    public void SetPlayerWeapon(WeaponBase weapon)
    {
        Weapon = weapon;
    }
    public void EquipPlayerWeapon()
    {
        PlayerModel player = GameObject.FindWithTag("Player").GetComponent<PlayerModel>();
        GameObject weapon = Instantiate(Weapon.gameObject, GameObject.Find("FirePoint").transform);
        player.SetWeapon(weapon.GetComponent<WeaponBase>());
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
