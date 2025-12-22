using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;

    [SerializeField] private GameObject _loadOutUI;
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private GameObject _keyGuideUI;
    [SerializeField] private GameObject _pausePanel;


    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    public void OnClickLoadOutUI()
    {
        _lobbyManager.OpenUI(_loadOutUI);
    }

    public void OnClickStageSelectUI()
    {
        _lobbyManager.OpenUI(_stageSelectUI);
    }

    public void OnClickKeyGuideUI()
    {
        _lobbyManager.OpenUI(_keyGuideUI);
        _keyGuideUI.SetActive(true);
    }

    public void OnOpenKeyGuideUI()
    {
        _keyGuideUI.SetActive(true);
        _pausePanel.SetActive(false);
    }

    public void OnCloseKeyGuideUI()
    {
        _keyGuideUI.SetActive(false);
        _pausePanel.SetActive(true);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(state == GameState.Paused);
    }

    // UI Btn OnClick Events
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void GoToTitle()
    {
        GameManager.Instance.GoToTitle();
    }
}
