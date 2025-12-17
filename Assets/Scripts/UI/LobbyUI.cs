using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyManager _lobbyManager;

    [SerializeField] private GameObject _loadOutUI;
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private GameObject _keyGuideUI;

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
    }
}
