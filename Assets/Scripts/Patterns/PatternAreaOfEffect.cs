using System.Collections;
using UnityEngine;

public class PatternAreaOfEffect : PatternBase
{
    [Header("장판 패턴 속성")]
    [SerializeField, Tooltip("경고 장판 지속 시간")] private float _warningTime;
    [SerializeField, Tooltip("실제 공격 장판 지속 시간")] private float _lifeTime;
    [SerializeField, Tooltip("장판 데미지")] private int _damage;
    [SerializeField, Tooltip("데미지 틱 간격")] private float _tickInterval;

    private GameObject _warningImage;
    private GameObject _attackAOE;
    private GameObject _target;

    protected override void Awake()
    {
        base.Awake();
        _warningImage = transform.GetChild(0).gameObject;
        _attackAOE = transform.GetChild(1).gameObject;

        if (_attackAOE.TryGetComponent<AreaOfEffectController>(out var controller))
        {
            controller.Init(_damage, _tickInterval);
        }
    }

    void OnEnable()
    {
        _warningImage.SetActive(false);
        _attackAOE.SetActive(false);
    }

    public override void Init(GameObject target)
    {
        _target = target;
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        if (_target != null)
        {
            transform.position = _target.transform.position;
        }

        _warningImage.SetActive(true);
        yield return new WaitForSeconds(_warningTime);
        _warningImage.SetActive(false);

        yield return new WaitForSeconds(0.8f);

        _attackAOE.SetActive(true);
        yield return new WaitForSeconds(_lifeTime);
        _attackAOE.SetActive(false);

        _isPatternActive = false;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        if (_warningImage != null) _warningImage.SetActive(false);
        if (_attackAOE != null) _attackAOE.SetActive(false);

        StopAllCoroutines();
    }
}