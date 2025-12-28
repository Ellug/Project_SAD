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
    public int UnlockStage { get; private set; }
    public int CurEnterStage { get; private set; }

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused  => CurrentState == GameState.Paused;
    public bool IsResult  => CurrentState == GameState.Result;

    public event Action<GameState> OnGameStateChanged;

    private GameObject _fadeInOutEffect;

    protected override void Awake()
    {
        base.Awake();
        UnlockStage = 1;
        CurEnterStage = 0;
    }

    private void Start()
    {
        _fadeInOutEffect = GameObject.Find("SceneChangeEffect");
        if (_fadeInOutEffect != null)
        {
            _fadeInOutEffect.SetActive(false);
            DontDestroyOnLoad(_fadeInOutEffect);
        }
    }

    public void PlayerWin()
    {
        IsPlayerWin = true;
        if (CurEnterStage == UnlockStage)
            UnlockStage++;
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

    public void EnterTheStage(int stage)
    {
        CurEnterStage = stage;
        string sceneName = "Stage" + stage.ToString();

        ChangeSceneWithFadeEffect(sceneName);
    }

    public void GoToLobby()
    {
        SetState(GameState.Playing);
        ChangeSceneWithFadeEffect("Lobby");
    }

    public void GoToTitle()
    {
        SetState(GameState.Playing);
        ChangeSceneWithFadeEffect("Title");
    }

    public void ReloadCurrentScene()
    {
        SetState(GameState.Playing);
        ChangeSceneWithFadeEffect(SceneManager.GetActiveScene().name);
    }

    private void ChangeSceneWithFadeEffect(string scene, bool animation = true)
    {
        SceneManager.LoadScene(scene);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
