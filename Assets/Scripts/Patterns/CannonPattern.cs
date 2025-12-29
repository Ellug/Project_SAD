using System.Collections;
using UnityEngine;

public class CannonPattern : PatternBase
{
    [Header("투사체 패턴 속성")]
    [SerializeField] private BulletBase _bulletPrefab;
    [SerializeField] private float _shootInterval = 0.1f;
    [SerializeField] private int _shootBulletNumber;
    [SerializeField] private Transform _spawnPosition;

    [Header("경고 장판")]
    [SerializeField] private ParticleSystem _WarnningArea;
    [SerializeField] private float _WarnningTime;
    [SerializeField] private float _WarnningFNTime;
    [SerializeField] private float _WarnningMaxLength;
    [SerializeField] private float _WarnningWidth = 5f;
    [SerializeField] private LayerMask _Layer;

    [Header("길이 배율 조정")]
    [Tooltip("장판이 너무 길면 이 값을 줄이세요 (예: 0.1)")][SerializeField] private float _lengthScaleModifier = 1f;

    private ParticleSystem _Warnning;
    private Transform _WarnningTransform;
    private GameObject _target;
    private WaitForSeconds _delay;
    private bool _isChasing = false;

    protected override void Awake()
    {
        base.Awake();
        _delay = new WaitForSeconds(_shootInterval);
    }

    void Update()
    {
        if (_isChasing && _WarnningTransform != null && _target != null)
        {
            UpdateLaserLogic();
        }
    }

    private void UpdateLaserLogic()
    {
        Vector3 origin = _spawnPosition.position;
        origin.y = 0f;

        Vector3 targetPos = _target.transform.position;
        targetPos.y = 0f;

        Vector3 direction = (targetPos - origin).normalized;
        if (direction == Vector3.zero) return;

        float currentDistance = _WarnningMaxLength;
        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, _WarnningMaxLength, _Layer))
        {
            currentDistance = hit.distance;
        }

        _WarnningTransform.rotation = Quaternion.LookRotation(direction);

        float finalScaleZ = currentDistance * _lengthScaleModifier;
        _WarnningTransform.localScale = new Vector3(_WarnningWidth, 1f, finalScaleZ);

        _WarnningTransform.position = origin + (direction * (currentDistance * 0.5f));
    }

    public void WarnningEffect()
    {
        if (_WarnningArea == null) return;

        Vector3 spawnPos = _spawnPosition.position;
        spawnPos.y = 0f;

        _Warnning = PoolManager.Instance.Spawn(_WarnningArea, spawnPos, Quaternion.identity);
        _WarnningTransform = _Warnning.transform;

        _isChasing = true;
        _Warnning.Clear();
        _Warnning.Play();

        StartCoroutine(PatternSequence());
    }

    private IEnumerator PatternSequence()
    {
        yield return new WaitForSeconds(_WarnningTime);
        _isChasing = false;
        yield return new WaitForSeconds(_WarnningFNTime);

        if (_Warnning != null)
        {
            _Warnning.Stop();
            PoolManager.Instance.Despawn(_Warnning.gameObject);
            _Warnning = null;
        }

        LaunchBullets();
    }

    private void LaunchBullets()
    {
        PlayPatternSound(PatternEnum.Cannon);

        Vector3 dir = (_target.transform.position - _spawnPosition.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            StartCoroutine(ShootRoutine(Quaternion.LookRotation(dir)));
    }

    private IEnumerator ShootRoutine(Quaternion rotation)
    {
        for (int i = 0; i < _shootBulletNumber; i++)
        {
            PoolManager.Instance.Spawn(_bulletPrefab, _spawnPosition.position, rotation);
            yield return _delay;
        }
    }

    public override void Init(GameObject target) => _target = target;
    protected override void PatternLogic() => WarnningEffect();
}