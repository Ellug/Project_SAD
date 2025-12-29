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
    private bool chase = false;

    private Vector3 _debugFirstPos;
    private Vector3 _debugSecondPos;
    private bool _showDebug = false;

    void Update()
    {
        if (chase && _Warnning != null && _predictiveAim != null)
        {
            _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    public void WarnningEffect()
    {
        if (_WarnningArea == null) return;

        _showDebug = false;
        _Warnning = Instantiate(_WarnningArea);
        _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        chase = true;

        _Warnning.Play();
        StartCoroutine(ChaseRoutine());
    }

    private IEnumerator ChaseRoutine()
    {
        yield return new WaitForSeconds(_WarnningTime);

        chase = false;

        if (_Warnning != null)
        {
            Vector3 stopPos = _Warnning.transform.position;
            Destroy(_Warnning.gameObject);

            if (_StaticWarnningArea != null)
            {
                _StaticWarnning = Instantiate(_StaticWarnningArea, stopPos, Quaternion.identity);
                _StaticWarnning.Play();
            }
        }

        yield return new WaitForSeconds(_WarnningDTime);

        if (_StaticWarnning != null) Destroy(_StaticWarnning.gameObject);

        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        Vector3 explosionPos = (_StaticWarnning != null) ? _StaticWarnning.transform.position : transform.position;
        _debugFirstPos = explosionPos;
        _showDebug = true;

        SpawnExplosion(_FirstExplosionParticle, explosionPos);
        CheckDamage(explosionPos);

        yield return new WaitForSeconds(_ExpolsionTime);

        _debugSecondPos = explosionPos;
        SpawnExplosion(_SecondExplosionParticle, explosionPos);
        CheckDamage(explosionPos);
    }

    private void SpawnExplosion(ParticleSystem particlePrefab, Vector3 position)
    {
        if (particlePrefab == null) return;

        ParticleSystem exp = Instantiate(particlePrefab, position, Quaternion.identity);
        var main = exp.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        exp.Play();
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

    protected override void PatternLogic() 
    {
        WarnningEffect();
    }

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }
}