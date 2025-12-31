using UnityEngine;

public class SettingButton : DOTweenUI
{
    [Header("Sub Panel")]
    [SerializeField] private GameObject _soundSettingPanel;
    [SerializeField] private GameObject _graphicSettingPanel;

    private void Awake()
    {
        _subPanel = new GameObject[] { _soundSettingPanel, _graphicSettingPanel };
    }

    //SettingMenuPanel
    public void OnClickSoundSetting()
    {
        _mainPanel.SetActive(false);
        _soundSettingPanel.SetActive(true);
    }

    public void OnClickGraphicSetting()
    {
        _mainPanel.SetActive(false);
        _graphicSettingPanel.SetActive(true);
    }

    public void OnClickReturn()
    {
        _soundSettingPanel.SetActive(false);
        _graphicSettingPanel.SetActive(false);
        _mainPanel.SetActive(true);
    }
}
