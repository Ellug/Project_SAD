using System.Collections;
using UnityEngine;

public class FlamethrowerPattern : PatternBase
{
    [Header("화염 방사 설정")]
    [SerializeField, Tooltip("화염방사 프리팹")] private Flamethrower _flamePrefab;
    [SerializeField, Tooltip("스폰 포인트")] private Transform _spawnPosition;
    [SerializeField, Tooltip("화염 유지 시간")] private float _fireDuration = 5.0f;

    [Header("경고 장판")]
    [SerializeField, Tooltip("경고 장판 프리팹")] private ParticleSystem _WarningAreaPrefab;
    [SerializeField, Tooltip("경고 장판 추적 시간")] private float _WarningTime = 1.5f;
    [SerializeField, Tooltip("추적 정지 후 발사까지 대기 시간")] private float _WarningFNTime = 0.2f;
    [SerializeField, Tooltip("장판 너비")] private float _WarningWidth = 10f;
    [SerializeField, Tooltip("장판 길이")] private float _WarningLength = 15f;
    [SerializeField, Tooltip("장판 시작점 오프셋 비율")] private float _WarningOffsetRate = 0.5f;

    private GameObject _target;
    private ParticleSystem _currentWarning;
    private Transform _warningTransform;
    private Flamethrower _activeFlame;
    private Quaternion _finalRotation;

    public override void Init(GameObject target) => _target = target;

    protected override void Update()
    {
        if (_isPatternActive && _warningTransform != null && _target != null)
        {
            UpdateWarningLogic();
        }
    }

    private void UpdateWarningLogic()
    {
        Vector3 dir = (_target.transform.position - _spawnPosition.position);
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            _warningTransform.rotation = Quaternion.Slerp(_warningTransform.rotation, targetRot, Time.deltaTime * 50f);
        }

        Vector3 forwardOffset = _warningTransform.forward * (_WarningLength * _WarningOffsetRate);
        _warningTransform.position = _spawnPosition.position + forwardOffset;
        _warningTransform.localScale = new Vector3(_WarningWidth, 1f, _WarningLength);
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarningAreaPrefab == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_WarningAreaPrefab, _spawnPosition.position, Quaternion.identity);
        _warningTransform = _currentWarning.transform;

        _currentWarning.Clear();
        _currentWarning.Play();

        _isPatternActive = true;

        yield return new WaitForSeconds(_WarningTime);

        _isPatternActive = false;
        _finalRotation = _warningTransform.rotation;

        yield return new WaitForSeconds(_WarningFNTime);

        RemoveWarning();
        Fire();

        yield return new WaitForSeconds(_fireDuration);

        CleanupPattern();
    }

    private void Fire()
    {
        if (_flamePrefab == null) return;

        PlayPatternSound(PatternEnum.Flamethrower);
        _activeFlame = PoolManager.Instance.Spawn(_flamePrefab, _spawnPosition.position, _finalRotation);

        if (_activeFlame != null)
        {
            _activeFlame.Init(_target);
            foreach (ParticleSystem ps in _activeFlame.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Clear();
                ps.Play();
            }
        }
    }

    private void RemoveWarning()
    {
        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(_currentWarning.gameObject);

            _currentWarning = null;
            _warningTransform = null;
        }
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
        RemoveWarning();

        if (_activeFlame != null)
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(_activeFlame.gameObject);
            _activeFlame = null;
        }
    }
}