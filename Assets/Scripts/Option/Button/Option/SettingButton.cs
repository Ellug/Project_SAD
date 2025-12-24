using UnityEngine;
using DG.Tweening;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private GameObject _uiAnimation;
    [SerializeField] private GameObject _settingMenuPanel;
    [SerializeField] private GameObject _soundSettingPanel;
    [SerializeField] private GameObject _graphicSettingPanel;

    private void OnEnable()
    {
        PlayOpenAnimation();
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
        PlayCloseAnimation();
    }


    private void PlayOpenAnimation()
    {
        Sequence seq = DOTween.Sequence();
        _soundSettingPanel.SetActive(false);
        _graphicSettingPanel.SetActive(false);
        _uiAnimation.transform.localScale = new Vector3(0f, 1f, 1f);
        _settingMenuPanel.transform.localScale = new Vector3(1f, 0f, 1f);
        seq.Append(_uiAnimation.transform.DOScaleX(1f, 0.3f).SetEase(Ease.InCubic));
        seq.Append(_uiAnimation.transform.DOScale(0f, 0f));
        seq.Append(_settingMenuPanel.transform.DOScaleY(1f, 0.1f));
    }

    private void PlayCloseAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_settingMenuPanel.transform.DOScaleY(0f, 0.1f));
        seq.Append(_uiAnimation.transform.DOScale(1f, 0f));
        seq.Append(_uiAnimation.transform.DOScaleX(0f, 0.4f).SetEase(Ease.OutQuart));
        seq.OnComplete(() => { 
            UIManager.Instance.CloseTopUI(); 
        });
    }
}
