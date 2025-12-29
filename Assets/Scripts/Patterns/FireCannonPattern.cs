using System.Collections;
using UnityEngine;

public class FireCannonPattern : PatternBase
{
    [Header("화염포 설정")]
    [Tooltip("화염포 프리팹")][SerializeField] private FireCannon _FireCannonPrefab;
    [Tooltip("화염포 생성 위치")][SerializeField] private Transform _SpawnPoint;

    [Header("경고 장판")]
    [Tooltip("경고 장판 프리팹")][SerializeField] private ParticleSystem _WarnningAreaPrefab;
    [Tooltip("경고 장판 추적 시간")][SerializeField] private float _WarnningTime = 2.0f;
    [Tooltip("정지 후 발사까지 대기 시간")][SerializeField] private float _WarnningFNTime = 0.5f;
    [Tooltip("장판 최대 사거리")][SerializeField] private float _WarnningMaxLength = 20f;
    [Tooltip("장판 너비")][SerializeField] private float _WarnningWidth = 5f;
    [Tooltip("장판 충돌 레이어")][SerializeField] private LayerMask _ObstacleLayer;
    [Tooltip("장판 길이 보정 배율")][SerializeField] private float _lengthScaleModifier = 1f;
    [Tooltip("플레이어 위치 예측 강도")][SerializeField] private float _predictiveOffset = 1.2f;

    private GameObject _player;
    private ParticleSystem _currentWarnning;
    private Transform _warnningTransform;
    private PredictiveAim _predictiveAim;
    private bool Chase = false;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void PatternLogic()
    {
        StartCoroutine(PatternSequence());
    }

    void Update()
    {
        if (Chase && _warnningTransform != null && _player != null)
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

        float currentDistance = _WarnningMaxLength;
        Ray ray = new Ray(origin, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, _WarnningMaxLength, _ObstacleLayer))
        {
            currentDistance = hit.distance;
        }

        _warnningTransform.rotation = Quaternion.LookRotation(direction);
        float finalScaleZ = currentDistance * _lengthScaleModifier;
        _warnningTransform.localScale = new Vector3(_WarnningWidth, 1f, finalScaleZ);
        _warnningTransform.position = origin + (direction * (currentDistance * 0.5f));
    }

    private IEnumerator PatternSequence()
    {
        if (_WarnningAreaPrefab == null) yield break;

        _currentWarnning = PoolManager.Instance.Spawn(_WarnningAreaPrefab, _SpawnPoint.position, Quaternion.identity);
        _warnningTransform = _currentWarnning.transform;
        Chase = true;
        _currentWarnning.Clear();
        _currentWarnning.Play();

        yield return new WaitForSeconds(_WarnningTime);

        Chase = false;

        yield return new WaitForSeconds(_WarnningFNTime);

        Fire();

        if (_currentWarnning != null)
        {
            _currentWarnning.Stop();
            PoolManager.Instance.Despawn(_currentWarnning.gameObject);
            _currentWarnning = null;
            _warnningTransform = null;
        }
    }

    private void Fire()
    {
        if (_FireCannonPrefab == null || _warnningTransform == null) return;

        Quaternion fireRotation = _warnningTransform.rotation;
        PoolManager.Instance.Spawn(_FireCannonPrefab, _SpawnPoint.position, fireRotation);
    }

    private void OnDestroy()
    {
        if (_currentWarnning != null && PoolManager.Instance != null)
        {
            PoolManager.Instance.Despawn(_currentWarnning.gameObject);
        }
    }
}