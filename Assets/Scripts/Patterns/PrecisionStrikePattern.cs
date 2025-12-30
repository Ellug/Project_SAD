using System.Collections;
using UnityEngine;

public class PrecisionStrikePattern : PatternBase
{
    [Header("경고 장판")]
    [Tooltip("추적용 경고 파티클")][SerializeField] private ParticleSystem _WarnningArea;
    [Tooltip("정지용 경고 파티클")][SerializeField] private ParticleSystem _StaticWarnningArea;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] private float _WarnningTime;
    [Tooltip("경고 장판 정지 후 대기 시간")][SerializeField] private float _WarnningDTime;

    [Header("폭발 설정")]
    [Tooltip("첫 번째 폭발 파티클")][SerializeField] private ParticleSystem _FirstExplosionParticle;
    [Tooltip("두 번째 폭발 파티클")][SerializeField] private ParticleSystem _SecondExplosionParticle;
    [Tooltip("생성 위치 예측 오프셋")][SerializeField] private float _ChaseOffset;
    [Tooltip("폭발 사이의 간격 시간")][SerializeField] private float _ExpolsionTime;
    [Tooltip("폭발 판정 범위")][SerializeField] private float _ExplosionRange;

    [Header("데미지")]
    [Tooltip("데미지")][SerializeField] private float _Dmg;

    private PredictiveAim _predictiveAim;
    private GameObject Player;
    private ParticleSystem _Warnning;
    private ParticleSystem _StaticWarnning;
    private Vector3 _lastStaticPos;

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void Update()
    {
        if (_isPatternActive && _Warnning != null && _predictiveAim != null)
        {
            _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarnningArea == null) yield break;

        _Warnning = PoolManager.Instance.Spawn(_WarnningArea, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        _Warnning.Clear();
        _Warnning.Play();

        _isPatternActive = true;

        yield return new WaitForSeconds(_WarnningTime);

        _isPatternActive = false;

        if (_Warnning != null)
        {
            Vector3 stopPos = _Warnning.transform.position;
            _lastStaticPos = stopPos;

            _Warnning.Stop();
            PoolManager.Instance.Despawn(_Warnning.gameObject);
            _Warnning = null;

            if (_StaticWarnningArea != null)
            {
                _StaticWarnning = PoolManager.Instance.Spawn(_StaticWarnningArea, stopPos, Quaternion.identity);
                _StaticWarnning.Clear();
                _StaticWarnning.Play();
            }
        }

        yield return new WaitForSeconds(_WarnningDTime);

        if (_StaticWarnning != null)
        {
            _lastStaticPos = _StaticWarnning.transform.position;
            _StaticWarnning.Stop();
            PoolManager.Instance.Despawn(_StaticWarnning.gameObject);
            _StaticWarnning = null;
        }

        yield return ExplosionSequence();
    }

    private IEnumerator ExplosionSequence()
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
        exp.Clear();
        exp.Play();

        StartCoroutine(DelayedDespawn(exp.gameObject, exp.main.duration + exp.main.startLifetime.constantMax));
    }

    private IEnumerator DelayedDespawn(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && obj.activeSelf)
        {
            PoolManager.Instance.Despawn(obj);
        }
    }

    private void CheckDamage(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _ExplosionRange);
        foreach (var hit in colliders)
        {
            if (hit.gameObject == Player)
            {
                if (Player.TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_Dmg);
                }
                break;
            }
        }
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        if (_Warnning != null)
        {
            _Warnning.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(_Warnning.gameObject);
            _Warnning = null;
        }

        if (_StaticWarnning != null)
        {
            _StaticWarnning.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(_StaticWarnning.gameObject);
            _StaticWarnning = null;
        }
    }
}