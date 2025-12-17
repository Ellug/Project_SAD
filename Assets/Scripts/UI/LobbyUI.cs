using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject _lobbyUI;
    [SerializeField] private GameObject _loadOutUI;
    [SerializeField] private GameObject _stageSelectUI;
    [SerializeField] private GameObject _keyGuideUI;

    public void OnClickLoadOutUI()
    {
        SetAllInActive();
        _loadOutUI.SetActive(true);
    }

    public void OnClickStageSelectUI()
    {
        SetAllInActive();
        _stageSelectUI.SetActive(true);
    }

    public void OnClickKeyGuideUI()
    {
        SetAllInActive();
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
