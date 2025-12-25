using UnityEngine;
using TMPro;

public class StageStaticUI : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameResultUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private TextMeshProUGUI _curStage;


    void Start()
    {
        UIManager.Instance.PauseUItrigger += OnOpenPausePanel;
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.PauseUItrigger -= OnOpenPausePanel;
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
