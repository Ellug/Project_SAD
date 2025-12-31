using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _keyGuidePanel;
    [SerializeField] private GameObject _creditPanel;

    public void OnClickGameStart()
    {
        GameManager.Instance.GoToLobby();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.OpenUI(_settingPanel);
    }

    public void OnClickKeyGuide()
    {
        UIManager.Instance.OpenUI(_keyGuidePanel);
    }

    public void OnClickCredit()
    {
        UIManager.Instance.OpenUI(_creditPanel);
    }

    public void OnClickExit()
    {
        GameManager.Instance.GameExit();
    }
}