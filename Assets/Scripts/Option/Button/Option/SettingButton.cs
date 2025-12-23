using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingMenuPanel;
    [SerializeField] private GameObject _soundSettingPanel;
    [SerializeField] private GameObject _graphicSettingPanel;

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

    public void OnClickReturn()
    {
        CloseAll();
        _settingMenuPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        CloseAll();
        gameObject.SetActive(false);
    }

    private void CloseAll()
    {
        _settingMenuPanel.SetActive(false);
        _soundSettingPanel.SetActive(false);
        _graphicSettingPanel.SetActive(false);
    }
}
