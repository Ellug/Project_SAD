using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _keyGuidePanel;
    [SerializeField] private GameObject _creditPanel;
    [SerializeField] private GameObject _settingMenuPanel;

    public void OnClickGameStart()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickSetting()
    {
        CloseAll();
        _settingPanel.SetActive(true);
        _settingMenuPanel.SetActive(true);
    }

    public void OnClickKeyGuide()
    {
        CloseAll();
        _settingPanel.SetActive(true);
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
        Application.Quit();
    }

    private void CloseAll()
    {
        _titlePanel.SetActive(false);
        _settingPanel.SetActive(false);
        _keyGuidePanel.SetActive(false);
        _creditPanel.SetActive(false);
    }
}