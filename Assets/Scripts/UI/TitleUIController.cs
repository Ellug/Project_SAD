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
        _settingPanel.SetActive(true);
    }

    public void OnClickKeyGuide()
    {
        _keyGuidePanel.SetActive(true);
    }

    public void OnClickCredit()
    {
        CloseAll();
        _creditPanel.SetActive(true);
    }

    public void OnClickBackToTitle()
    {
        CloseAll();
        _titlePanel.SetActive(true);
    }

    public void OnClickExit()
    {
        GameManager.Instance.GameExit();
    }

    private void CloseAll()
    {
        _titlePanel.SetActive(false);
        _settingPanel.SetActive(false);
        _keyGuidePanel.SetActive(false);
        _creditPanel.SetActive(false);
    }
}