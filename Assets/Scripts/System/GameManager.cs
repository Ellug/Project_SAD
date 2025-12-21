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

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused  => CurrentState == GameState.Paused;
    public bool IsResult  => CurrentState == GameState.Result;

    public event Action<GameState> OnGameStateChanged;

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

    public void GoToTitle()
    {
        SetState(GameState.Playing);
        SceneManager.LoadScene("Title");
    }

    public void ReloadCurrentScene()
    {
        SetState(GameState.Playing);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
