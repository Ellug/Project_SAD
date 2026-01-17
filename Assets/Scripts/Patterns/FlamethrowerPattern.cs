using System.Collections;
using UnityEngine;

public class FlamethrowerPattern : PatternBase
{
    [Header("화염 방사 설정")]
    [SerializeField, Tooltip("화염방사 프리팹")] private Flamethrower _flamePrefab;
    [SerializeField, Tooltip("스폰 포인트")] private Transform _spawnPosition;
    [SerializeField, Tooltip("화염 유지 시간")] private float _fireDuration = 5.0f;

    [Header("화염 물리 수치")]
    [SerializeField, Tooltip("화염방사 총 길이")] private float _Distance = 15.0f;
    [SerializeField, Tooltip("시작 지점 반지름")] private float _StartRadius = 0.1f;
    [SerializeField, Tooltip("끝 지점 반지름")] private float _EndRadius = 2.0f;
    [SerializeField, Tooltip("구체 간 밀도")] private float _Density = 1.8f;
    [SerializeField, Tooltip("데미지")] private float _Dmg = 10.0f;

    [Header("화상 디버프 설정")]
    [SerializeField, Tooltip("화상 지속시간")] private float _BurnDebuffTime = 2.0f;
    [SerializeField, Tooltip("화상 데미지")] private float _BurnDmg = 5.0f;
    [SerializeField, Tooltip("화상 틱")] private float _TickInterval = 0.5f;

    private Flamethrower _activeFlame;

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Quaternion finalRotation;
        if (_useFixedSpawnPoint)
        {
            finalRotation = _spawnPosition.rotation;
        }
        else
        {
            finalRotation = _lastDirection != Vector3.zero ? Quaternion.LookRotation(_lastDirection) : _spawnPosition.rotation;
        }

        RemoveWarning();
        Fire(finalRotation);

        yield return new WaitForSeconds(_fireDuration);

        CleanupPattern();
    }

    private void Fire(Quaternion rotation)
    {
        if (_flamePrefab == null || _spawnPosition == null) return;

        PlayPatternSound(PatternEnum.Flamethrower);
        _activeFlame = PoolManager.Instance.Spawn(_flamePrefab, _spawnPosition.position, rotation);

        if (_activeFlame != null)
        {
            _activeFlame.Init(_target, _Distance, _StartRadius, _EndRadius, _Density, _fireDuration, _Dmg, _BurnDmg, _BurnDebuffTime, _TickInterval);

            foreach (ParticleSystem ps in _activeFlame.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Clear();
                ps.Play();
            }
        }
    }

    protected override void CleanupPattern()
    {
        if (_activeFlame != null)
        {
            if (PoolManager.Instance != null)
                PoolManager.Instance.Despawn(_activeFlame.gameObject);
            _activeFlame = null;
        }
    }
}