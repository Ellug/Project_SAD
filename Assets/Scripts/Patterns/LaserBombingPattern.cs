using System.Collections;
using UnityEngine;

public class MultiLaserBombingPattern : PatternBase
{
    [Header("경고 장판")]
    [SerializeField, Tooltip("경고 장판 프리팹")] private ParticleSystem _warningAreaPrefab;
    [SerializeField, Tooltip("경고 장판 추적 시간")] private float _chaseTime = 2.0f;
    [SerializeField, Tooltip("경고 장판 고정 시간")] private float _readyTime = 0.5f;
    [SerializeField, Tooltip("경고 장판 이격 거리")] private float _chaseOffset = 0.2f;

    [Header("레이저 포격 설정")]
    [SerializeField, Tooltip("레이저 프리팹")] private ParticleSystem _laserParticlePrefab;
    [SerializeField, Tooltip("레이저 포격 갯수")] private int _laserCount = 5;
    [SerializeField, Tooltip("레이저 포격 반경")] private float _bombingRadius = 3.0f;
    [SerializeField, Tooltip("레이저 포격 간격")] private float _intervalBetweenLasers = 0.1f;
    [SerializeField, Tooltip("레이저 폭발 판정 범위")] private float _explosionRange = 1.5f;
    [SerializeField, Tooltip("데미지")] private float _damage = 10f;

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
            _currentWarning.transform.position = _predictiveAim.PredictiveAimCalc(_chaseOffset);
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_warningAreaPrefab == null || _predictiveAim == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_warningAreaPrefab, _predictiveAim.PredictiveAimCalc(_chaseOffset), Quaternion.identity);
        _currentWarning.Clear();
        _currentWarning.Play();

        _isPatternActive = true;
        yield return new WaitForSeconds(_chaseTime);

        _isPatternActive = false;
        Vector3 finalCenterPos = _currentWarning.transform.position;

        yield return new WaitForSeconds(_readyTime);

        RemoveWarning();

        for (int i = 0; i < _laserCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _bombingRadius;
            Vector3 firePos = finalCenterPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            SpawnLaser(firePos);
            PlayPatternSound(PatternEnum.LaserBombing);
            CheckDamage(firePos);

            yield return new WaitForSeconds(_intervalBetweenLasers);
        }
    }

    private void SpawnLaser(Vector3 position)
    {
        if (_laserParticlePrefab == null) return;

        ParticleSystem laser = PoolManager.Instance.Spawn(_laserParticlePrefab, position, Quaternion.identity);
        laser.Clear();
        laser.Play();
    }

    private void CheckDamage(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _explosionRange);
        foreach (var hit in colliders)
        {
            if (hit.gameObject == _player)
            {
                if (_player.TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_damage);
                }
                break;
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
        }
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
        RemoveWarning();
    }
}