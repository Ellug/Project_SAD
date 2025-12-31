using System.Collections;
using UnityEngine;

public class IceAreaPattern : PatternBase
{
    [Header("경고 장판")]
    [Tooltip("경고 파티클")][SerializeField] private ParticleSystem _WarnningArea;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] private float _WarnningTime = 2.0f;
    [Tooltip("경고 장판 정지 후 대기 시간")][SerializeField] private float _WarnningDTime = 0.5f;

    [Header("냉기 장판 생성")]
    [Tooltip("냉기 장판 프리팹")][SerializeField] private IceArea _IceAreaPrefab;
    [Tooltip("생성 위치 예측 오프셋")][SerializeField] private float _ChaseOffset = 0.2f;

    private PredictiveAim _predictiveAim;
    private GameObject _player;
    private ParticleSystem _currentWarning;

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
        if (_WarnningArea == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_WarnningArea, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        _currentWarning.Clear();
        _currentWarning.Play();

        _isPatternActive = true;

        yield return new WaitForSeconds(_WarnningTime);

        _isPatternActive = false;
        Vector3 spawnPos = _currentWarning != null ? _currentWarning.transform.position : _predictiveAim.PredictiveAimCalc(_ChaseOffset);

        yield return new WaitForSeconds(_WarnningDTime);

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
        }

        SpawnIceArea(spawnPos);
    }

    private void SpawnIceArea(Vector3 position)
    {
        if (_IceAreaPrefab == null) return;

        PlayPatternSound(PatternEnum.IceArea);
        IceArea ice = PoolManager.Instance.Spawn(_IceAreaPrefab, position, Quaternion.identity);

        if (ice != null)
        {
            ice.Init(_player);
        }
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