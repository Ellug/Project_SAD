using UnityEngine;
public class DummyController : BossController
{
    [SerializeField] private GameObject _guideUi;

    private Camera _mainCam;
    private Vector3 _posOffset;

    protected override void Start()
    {
        base.Start();
        _mainCam = Camera.main;
        _posOffset = new Vector3(40f, 120f, 0f);
        UIManager.Instance.FirstUItrigger += DeactiveUI;
        UIManager.Instance.AllUIClosed += ActiveUI;
    }

    protected void FixedUpdate()
    {
        GuideUiPositionUpdate();
    }

    private void OnDestroy()
    {
        UIManager.Instance.FirstUItrigger -= DeactiveUI;
        UIManager.Instance.AllUIClosed -= ActiveUI;
    }

    public new void TakeDamage(float dmg, bool isCounterable)
    {
        _dmgFont.Spawn(transform.position, dmg);
    }
    private void GuideUiPositionUpdate()
    {
        Vector3 curPos = _mainCam.WorldToScreenPoint(transform.position);

        _guideUi.transform.position = curPos + _posOffset;
    }
    public void DeactiveUI()
    {
        _guideUi.SetActive(false);
    }
    public void ActiveUI()
    {
        _guideUi.SetActive(true);
    }
}
