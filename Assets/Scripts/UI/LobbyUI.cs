using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject _loadOutUI;
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _keyGuideUI;
    [SerializeField] private GameObject _pausePanel;

    private void Start()
    {
        UIManager.Instance.PauseUItrigger += OnOpenPausePanel;
    }

    private void OnDestroy()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.PauseUItrigger -= OnOpenPausePanel;
    }

    public void OnClickLoadOutUI()
    {
        UIManager.Instance.OpenUI(_loadOutUI);
    }

    public void OnClickStageSelectUI()
    {
        UIManager.Instance.OpenUI(_stageSelectUI);
    }

    public void OnClickKeyGuideUI()
    {
        UIManager.Instance.OpenUI(_keyGuideUI);
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

    public void GoToTitle()
    {
        GameManager.Instance.GoToTitle();
    }
}
