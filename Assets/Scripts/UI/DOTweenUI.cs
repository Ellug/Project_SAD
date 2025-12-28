using DG.Tweening;
using UnityEngine;

public class DOTweenUI : MonoBehaviour
{
    [Header("Essential Object")]
    [SerializeField] protected GameObject _mainPanel;
    [Header("Select Option")]
    [SerializeField] protected GameObject _uiAnimation;
    [SerializeField] protected bool _simpleMode;
    [SerializeField] protected bool _leftRight;

    protected GameObject[] _subPanel;

    protected virtual void OnEnable()
    {
        PlayOpenAnimation();
    }

    public virtual void OnClickExit()
    {
        PlayCloseAnimation();
    }

    protected virtual void PlayOpenAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        if (_subPanel != null)
            foreach (GameObject subPanel in _subPanel)
                subPanel.SetActive(false);

        if (!_simpleMode)
        {
            _uiAnimation.SetActive(true);
            _uiAnimation.transform.localScale = new Vector3(0f, 1f, 1f);
            if (_leftRight)
            {
                _mainPanel.transform.localScale = new Vector3(0f, 1f, 1f);
                seq.Append(_uiAnimation.transform.DOScaleX(1f, 0.3f).SetEase(Ease.InCubic));
                seq.Append(_mainPanel.transform.DOScaleX(1f, 0.1f));
            }
            else
            {
                _mainPanel.transform.localScale = new Vector3(1f, 0f, 1f);
                seq.Append(_uiAnimation.transform.DOScaleX(1f, 0.3f).SetEase(Ease.InCubic));
                seq.Append(_mainPanel.transform.DOScaleY(1f, 0.1f));
            }

            seq.OnComplete(() => {
                _uiAnimation.SetActive(false);
            });
        }
        else
        {
            _mainPanel.transform.localScale = new Vector3(1f, 0f, 1f);
            seq.Append(_mainPanel.transform.DOScaleY(1f, 0.1f));
        }
    }

    protected virtual void PlayCloseAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        if (!_simpleMode)
        {
            _uiAnimation.SetActive(true);
            seq.Append(_mainPanel.transform.DOScaleY(0f, 0.1f));
            seq.Append(_uiAnimation.transform.DOScaleX(0f, 0.4f).SetEase(Ease.OutQuart));
        }
        else
        {
            seq.Append(_mainPanel.transform.DOScaleY(0f, 0.1f));
        }   
        
        seq.OnComplete(() => {
            UIManager.Instance.CloseTopUI();
        });
    }
}
