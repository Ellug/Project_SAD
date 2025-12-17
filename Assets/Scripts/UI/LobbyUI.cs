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
        SetAllInActive();
        _lobbyManager.OpenUI(_keyGuideUI);
        _keyGuideUI.SetActive(true);
    }

    public void OnClickExit()
    {
        GameManager.Instance.EquipPlayerWeapon();
        SetAllInActive();
        _lobbyUI.SetActive(true);
    }
    
    public void SetAllInActive()
    {
        _lobbyUI.SetActive(false);
        _loadOutUI.SetActive(false);
        _stageSelectUI.SetActive(false);
        _keyGuideUI.SetActive(false);
    }
}
