using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

public enum GameState
{
    Playing,
    Paused,
    Result
}

public class GameManager : SingletonePattern<GameManager>
{
    [SerializeField] private GameObject _fadeInOutEffectPrefab;

    public GameState CurrentState { get; private set; } = GameState.Playing;
    public bool IsPlayerWin { get; private set; }
    public int UnlockStage { get; private set; }
    public int CurEnterStage { get; private set; }

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused  => CurrentState == GameState.Paused;
    public bool IsResult  => CurrentState == GameState.Result;

    public event Action<GameState> OnGameStateChanged;

    private CanvasGroup _fadeInOutEffect;

    protected override void Awake()
    {
        base.Awake();
        UnlockStage = 1;
        CurEnterStage = 0;
    }

    private void Start()
    {
        if (_fadeInOutEffectPrefab != null)
        {
            GameObject instance = Instantiate(_fadeInOutEffectPrefab);
            _fadeInOutEffect = instance.GetComponent<CanvasGroup>();
            instance.SetActive(false);
            DontDestroyOnLoad(instance);
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
        ChangeSceneWithFadeEffect("Lobby");
    }

    public void GoToTitle()
    {
        ChangeSceneWithFadeEffect("Title", false);
    }

    public void ReloadCurrentScene()
    {
        ChangeSceneWithFadeEffect(SceneManager.GetActiveScene().name);
    }

    private void ChangeSceneWithFadeEffect(string scene, bool animation = true)
    {
        if (animation && _fadeInOutEffect != null) 
        {
            StartCoroutine(FadeOutInLoading(scene));
        }
        else
        {
            SetState(GameState.Playing);
            SceneManager.LoadScene(scene);
        } 
    }

    private IEnumerator FadeOutInLoading(string scene)
    {
        SetState(GameState.Paused);

        // 비동기 로딩 시작
        AsyncOperation temp = SceneManager.LoadSceneAsync(scene);
        temp.allowSceneActivation = false;
        _fadeInOutEffect.gameObject.SetActive(true);

        // 애니메이션 재생이 완료될 때까지 대기함.
        yield return _fadeInOutEffect.DOFade(1f, 0.5f).SetUpdate(true).WaitForCompletion();

        // 진행도에 따라 대기함 (규모가 작아서 대기할 일이 거의 없음)
        while (temp.progress < 0.9f) 
            yield return null;
        temp.allowSceneActivation = true;

        // 한 프레임 대기 : 이게 없으면 페이드 인 효과가 제대로 나타나지 않고 씬 시작 시 끊기는 느낌이 있음
        // 찾아보니 새로운 씬의 오브젝트들이 Awake, Start 등을 실행하도록 하기 위함이라고 함.
        yield return null;

        // 애니메이션 대기 후 나머지 처리
        yield return _fadeInOutEffect.DOFade(0f, 0.5f).SetUpdate(true).WaitForCompletion();
        _fadeInOutEffect.gameObject.SetActive(false);
        SetState(GameState.Playing);
    }

    public void GameExit()
    {
        Application.Quit();
    }
}
