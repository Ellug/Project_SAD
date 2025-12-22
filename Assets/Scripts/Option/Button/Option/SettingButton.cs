using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingPanel;
    [SerializeField] private GameObject _keyGuideUI;
    [SerializeField] private GameObject _settingMenuPanel;
    [SerializeField] private GameObject _eSCMenuPanel;
    [SerializeField] private GameObject _soundSettingPanel;
    [SerializeField] private GameObject _graphicSettingPanel;
    [SerializeField] private GameObject _titlePanel;

    //SettingMenuPanel
    public void OnClickSoundSetting()
    {
        CloseAll();
        _soundSettingPanel.SetActive(true);
    }
    public void OnClickGraphicSetting()
    {
        CloseAll();
        _graphicSettingPanel.SetActive(true);
    }

    //ESCMenuPanel
    public void OnClickSetting()
    {
        CloseAll();
        _settingMenuPanel.SetActive(true);
    }
    public void OnClickKeyGuide()
    {
        CloseAll();
        _keyGuideUI.SetActive(true);
    }
    public void OnClickMainScreen()
    {

    }

    public void OnClcikExit()
    {
        CloseAll();
        _settingPanel.SetActive(false);
        _titlePanel.SetActive(true);
    }

    private void CloseAll()
    {
        _keyGuideUI.SetActive(false);
        _settingMenuPanel.SetActive(false);
        _eSCMenuPanel.SetActive(false);
        _soundSettingPanel.SetActive(false);
        _graphicSettingPanel.SetActive(false);
    }
}
