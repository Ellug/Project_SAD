using System.Collections;
using UnityEngine;

public class FireCannonPattern : PatternBase
{
    [Header("화염포 설정")]
    [SerializeField, Tooltip("화염포 프리팹")] private FireCannon _FireCannonPrefab;
    [SerializeField, Tooltip("화염포 생성 위치")] private Transform _SpawnPoint;

    [Header("경고 장판")]
    [SerializeField, Tooltip("경고 장판 프리팹")] private ParticleSystem _WarnningAreaPrefab;
    [SerializeField, Tooltip("경고 장판 추적 시간")] private float _WarnningTime = 2.0f;
    [SerializeField, Tooltip("정지 후 발사까지 대기 시간")] private float _WarnningFNTime = 0.5f;
    [SerializeField, Tooltip("장판 최대 사거리")] private float _WarnningMaxLength = 20f;
    [SerializeField, Tooltip("장판 너비")] private float _WarnningWidth = 5f;
    [SerializeField, Tooltip("장판 충돌 레이어")] private LayerMask _ObstacleLayer;
    [SerializeField, Tooltip("장판 길이 보정 배율")] private float _lengthScaleModifier = 1f;
    [SerializeField, Tooltip("플레이어 위치 예측 강도")] private float _predictiveOffset = 1.2f;

    private GameObject _player;
    private ParticleSystem _currentWarning;
    private Transform _warningTransform;
    private PredictiveAim _predictiveAim;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void Update()
    {
        if (_isPatternActive && _warningTransform != null && _player != null)
        {
            UpdateWarningLogic();
        }
    }

    private void UpdateWarningLogic()
    {
        Vector3 origin = _SpawnPoint.position;
        origin.y = 0.5f;

        Vector3 targetPos = _predictiveAim.PredictiveAimCalc(_predictiveOffset);
        targetPos.y = 0.5f;

        Vector3 direction = (targetPos - origin).normalized;
        if (direction == Vector3.zero) return;

        float distance = _WarnningMaxLength;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, _WarnningMaxLength, _ObstacleLayer))
            distance = hit.distance;

        _warningTransform.rotation = Quaternion.LookRotation(direction);
        _warningTransform.localScale = new Vector3(_WarnningWidth, 1f, distance * _lengthScaleModifier);
        _warningTransform.position = origin + direction * (distance * 0.5f);
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarnningAreaPrefab == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_WarnningAreaPrefab, _SpawnPoint.position, Quaternion.identity);
        _warningTransform = _currentWarning.transform;
        _isPatternActive = true;

        _currentWarning.Clear();
        _currentWarning.Play();

        yield return new WaitForSeconds(_WarnningTime);
        _isPatternActive = false;

        yield return new WaitForSeconds(_WarnningFNTime);

        Fire();

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }
    }

    private void Fire()
    {
        if (_FireCannonPrefab == null || _warningTransform == null) return;

        PlayPatternSound(PatternEnum.FireCannon);
        PoolManager.Instance.Spawn(_FireCannonPrefab, _SpawnPoint.position, _warningTransform.rotation);
    }

    protected override void CleanupPattern()
    {
        if (_currentWarning != null && PoolManager.Instance != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }
    }
}
