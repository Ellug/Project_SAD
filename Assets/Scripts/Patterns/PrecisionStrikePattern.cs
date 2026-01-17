using System.Collections;
using UnityEngine;

public class PrecisionStrikePattern : PatternBase
{
    [Header("경고 장판 추가 설정")]
    [Tooltip("정지용 경고 파티클")][SerializeField] private ParticleSystem _StaticWarnningArea;

    [Header("폭발 설정")]
    [Tooltip("첫 번째 폭발 파티클")][SerializeField] private ParticleSystem _FirstExplosionParticle;
    [Tooltip("두 번째 폭발 파티클")][SerializeField] private ParticleSystem _SecondExplosionParticle;
    [Tooltip("폭발 사이의 간격 시간")][SerializeField] private float _ExpolsionTime;
    [Tooltip("폭발 판정 범위")][SerializeField] private float _ExplosionRange;

    [Header("데미지")]
    [Tooltip("데미지")][SerializeField] private float _Dmg;

    private ParticleSystem _StaticWarnning;
    private Vector3 _lastStaticPos;

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Vector3 stopPos;
        if (_useFixedSpawnPoint)
        {
            stopPos = transform.position;
        }
        else
        {
            stopPos = _warningTransform != null ? _warningTransform.position : transform.position;
        }

        _lastStaticPos = stopPos;

        RemoveWarning();

        if (_StaticWarnningArea != null)
        {
            _StaticWarnning = PoolManager.Instance.Spawn(_StaticWarnningArea, _lastStaticPos, Quaternion.identity);
            _StaticWarnning.Play();
        }

        yield return StartCoroutine(ExecuteExplosions());

        CleanupPattern();
    }

    private IEnumerator ExecuteExplosions()
    {
        Vector3 explosionPos = _lastStaticPos;

        SpawnExplosion(_FirstExplosionParticle, explosionPos);
        CheckDamage(explosionPos);

        PlayPatternSound(PatternEnum.PrecisionStrike);

        yield return new WaitForSeconds(_ExpolsionTime);

        SpawnExplosion(_SecondExplosionParticle, explosionPos);
        CheckDamage(explosionPos);
    }

    private void SpawnExplosion(ParticleSystem particlePrefab, Vector3 position)
    {
        if (particlePrefab == null) return;

        ParticleSystem exp = PoolManager.Instance.Spawn(particlePrefab, position, Quaternion.identity);
        if (exp != null)
        {
            exp.Clear();
            exp.Play();
        }
    }

    private void CheckDamage(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _ExplosionRange);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                if (hit.TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_Dmg);
                }
                break;
            }
        }
    }

    protected override void CleanupPattern()
    {
        if (_StaticWarnning != null)
        {
            _StaticWarnning.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(_StaticWarnning.gameObject);
            _StaticWarnning = null;
        }
        RemoveWarning();
    }
}