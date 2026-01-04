using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuickGuideUI : MonoBehaviour
{
    private Vector3 _appearPos;
    private Vector3 _hidePos;
    private bool _isAppeared;

    private void Awake()
    {
        RectTransform myPanel = GetComponent<RectTransform>();
        _appearPos = transform.position;
        _hidePos = _appearPos;
        _hidePos.y = (myPanel.rect.height * -1) + 50f;
        _isAppeared = true;
    }

    private void Start()
    {
        UIManager.Instance.RegistQuickGuide(this);        
    }

    public void PanelToggle()
    {
        if (_isAppeared)
            Hide();
        else
            Appear();

        _isAppeared = !_isAppeared;
    }

    private void Appear()
    {
        transform.DOMoveY(_appearPos.y, 0.8f).SetEase(Ease.OutCubic);
    }

    private void Hide()
    {
        transform.DOMoveY(_hidePos.y, 0.8f).SetEase(Ease.OutCubic);
    }
}
