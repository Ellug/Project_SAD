using System.Collections;
using UnityEngine;

public class PatternAreaOfEffect : PatternBase
{
    [SerializeField] private float _warningTime;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _damage;
    [SerializeField] private float _tickInterval;

    private GameObject _warningImage;
    private GameObject _attackAOE;
    private GameObject _target;

    void Awake()
    {
        base.Awake();
        _warningImage = transform.GetChild(0).gameObject;
        _attackAOE = transform.GetChild(1).gameObject;
        _attackAOE.GetComponent<AreaOfEffectController>()
            .Init(_damage, _tickInterval);
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

    protected override void PatternLogic()
    {
        StartCoroutine(AreaOfEffectLogic());
    }

    private IEnumerator AreaOfEffectLogic()
    {
        transform.position = _target.transform.position;
        _warningImage.SetActive(true);
        yield return new WaitForSeconds(_warningTime);
        _warningImage.SetActive(false);
        yield return new WaitForSeconds(0.8f);
        _attackAOE.SetActive(true);
        yield return new WaitForSeconds(_lifeTime);
        _attackAOE.SetActive(false);
    }
}
