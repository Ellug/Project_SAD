using UnityEngine;
using TMPro;

public class StageStaticUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _curStage;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _gameResultUI;
    [SerializeField] private GameObject _missionCompletePanel;
    [SerializeField] private GameObject _missionFailedPanel;

    void Start()
    {
        UIManager.Instance.PauseUItrigger += OnOpenPausePanel;
        GameManager.Instance.OnGameStateChanged += GameResultProcess;
        _curStage.text = $"STAGE {GameManager.Instance.CurEnterStage}";
    }

    void OnDestroy()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.PauseUItrigger -= OnOpenPausePanel;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= GameResultProcess;
    }

    // 스태틱 UI가 게임 결과에서 처리할 일.
    private void GameResultProcess(GameState state)
    {
        if (state != GameState.Result) return;

        if (_gameResultUI != null)
        {
            _gameResultUI.SetActive(true);

            bool isWin = GameManager.Instance.IsPlayerWin;

            if (isWin)
                _missionCompletePanel.SetActive(true);
            else 
                _missionFailedPanel.SetActive(true);
        }
    }

    public void OnClickSettingsUI()
    {
        UIManager.Instance.OpenUI(_settingsUI);
    }

    public void OnCloseUI()
    {
        UIManager.Instance.TogglePause();
    }

    private void OnOpenPausePanel()
    {
        if (_pausePanel != null)
        {
            UIManager.Instance.OpenUI(_pausePanel);
        }
    }

    public void Retry()
    {
        GameManager.Instance.ReloadCurrentScene();
    }

    public void GoToLobby()
    {
        GameManager.Instance.GoToLobby();
    }

    public void GameExit()
    {
        GameManager.Instance.GameExit();
    }
}
