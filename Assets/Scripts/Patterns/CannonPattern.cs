using System.Collections;
using UnityEngine;

public class CannonPattern : PatternBase
{
    [Header("투사체 패턴 속성")]
    [SerializeField, Tooltip("발사할 총알 프리팹")] private BulletBase _bulletPrefab;
    [SerializeField, Tooltip("총알 발사 간격")] private float _shootInterval = 0.1f;
    [SerializeField, Tooltip("발사할 총알 수")] private int _shootBulletNumber;
    [SerializeField, Tooltip("총알 생성 위치")] private Transform _spawnPosition;

    [Header("경고 장판")]
    [SerializeField, Tooltip("경고 장판 프리팹")] private ParticleSystem _WarnningArea;
    [SerializeField, Tooltip("경고 장판 추적 시간")] private float _WarnningTime;
    [SerializeField, Tooltip("경고 후 발사까지 지연 시간")] private float _WarnningFNTime;
    [SerializeField, Tooltip("장판 최대 길이")] private float _WarnningMaxLength;
    [SerializeField, Tooltip("장판 너비")] private float _WarnningWidth = 5f;
    [SerializeField, Tooltip("레이어 마스크")] private LayerMask _Layer;

    [Header("길이 배율 조정")]
    [SerializeField, Tooltip("장판 길이 보정 배율")] private float _lengthScaleModifier = 1f;

    private ParticleSystem _currentWarning;
    private Transform _warningTransform;
    private GameObject _target;
    private WaitForSeconds _shootDelay;
    private Vector3 dir

    protected override void Awake()
    {
        base.Awake();
        _shootDelay = new WaitForSeconds(_shootInterval);
    }

    protected override void Update()
    {
        if (_isPatternActive && _warningTransform != null && _target != null)
            UpdateWarningLogic();
    }

    private void UpdateWarningLogic()
    {
        Vector3 origin = _spawnPosition.position;
        origin.y = 0f;

        Vector3 targetPos = _target.transform.position;
        targetPos.y = 0f;

        Vector3 direction = (targetPos - origin).normalized;
        if (direction == Vector3.zero) return;

        float distance = _WarnningMaxLength;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _WarnningMaxLength, _Layer))
            distance = hit.distance;

        _warningTransform.rotation = Quaternion.LookRotation(direction);
        _warningTransform.localScale = new Vector3(_WarnningWidth, 1f, distance * _lengthScaleModifier);
        _warningTransform.position = origin + direction * (distance * 0.5f);
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarnningArea != null)
        {
            _currentWarning = PoolManager.Instance.Spawn(_WarnningArea, _spawnPosition.position, Quaternion.identity);
            _warningTransform = _currentWarning.transform;
            _isPatternActive = true;

            _currentWarning.Clear();
            _currentWarning.Play();
        }

        yield return new WaitForSeconds(_WarnningTime);
        _isPatternActive = false;
        yield return new WaitForSeconds(_WarnningFNTime);

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            dir = _currentWarning.transform.forward;
            dir.y = 0f;
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }

        if (dir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            for (int i = 0; i < _shootBulletNumber; i++)
            {
                PoolManager.Instance.Spawn(_bulletPrefab, _spawnPosition.position, rotation);
                yield return _shootDelay;
            }
        }

        PlayPatternSound(PatternEnum.Cannon);
    }

    protected override void CleanupPattern()
    {
        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }
    }

    public override void Init(GameObject target) => _target = target;
}
