using UnityEngine;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _settingMenuPanel;
    [SerializeField] private GameObject _soundSettingPanel;
    [SerializeField] private GameObject _graphicSettingPanel;

    private void OnEnable()
    {
        OnClickReturn();
    }

    //SettingMenuPanel
    public void OnClickSoundSetting()
    {
        _settingMenuPanel.SetActive(false);
        _soundSettingPanel.SetActive(true);
    }

    public void OnClickGraphicSetting()
    {
        _settingMenuPanel.SetActive(false);
        _graphicSettingPanel.SetActive(true);
    }

    public void OnClickReturn()
    {
        _soundSettingPanel.SetActive(false);
        _graphicSettingPanel.SetActive(false);
        _settingMenuPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        UIManager.Instance.CloseTopUI();
    }

    //private void CloseAll()
    //{
    //    _settingMenuPanel.SetActive(false);
    //    _soundSettingPanel.SetActive(false);
    //    _graphicSettingPanel.SetActive(false);
    //}
}
