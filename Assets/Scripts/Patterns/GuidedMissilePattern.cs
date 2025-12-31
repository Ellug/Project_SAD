using System.Collections;
using UnityEngine;

public class GuidedMissilePattern : PatternBase
{
    [Header("패턴 설정")]
    [Tooltip("미사일 프리팹")][SerializeField] private GuidedMissile _MissilePrefab;
    [Tooltip("미사일 생성 위치")][SerializeField] private GameObject _SpawnPoint;

    [Header("경고 장판")]
    [Tooltip("경고 파티클")][SerializeField] private ParticleSystem _WarningAreaPrefab;
    [Tooltip("예측 위치 오프셋")][SerializeField] private float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] private float _WarningTime;

    private ParticleSystem _currentWarning;
    private GameObject _player;
    private PredictiveAim _predictiveAim;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void Update()
    {
        if (_isPatternActive && _currentWarning != null && _predictiveAim != null)
        {
            _currentWarning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarningAreaPrefab == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_WarningAreaPrefab, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        _currentWarning.Clear();
        _currentWarning.Play();

        _isPatternActive = true;

        yield return new WaitForSeconds(_WarningTime);

        _isPatternActive = false;

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
        }

        MissileLaunch();
    }

    private void MissileLaunch()
    {
        if (_MissilePrefab == null || _SpawnPoint == null) return;

        PlayPatternSound(PatternEnum.GuidedMissile);
        PoolManager.Instance.Spawn(_MissilePrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
        }
    }
}