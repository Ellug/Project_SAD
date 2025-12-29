using System.Collections;
using UnityEngine;

public class SlowAreaPattern : PatternBase
{
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("슬로우 파티클")][SerializeField] ParticleSystem _SlowParticle;
    [Tooltip("슬로우 예고 장판")][SerializeField] ParticleSystem _WarinningParticle;
    [Tooltip("슬로우 장판 범위")][SerializeField] float _SlowRange;
    [Tooltip("슬로우 장판 지속시간")][SerializeField] float _SlowAreaTime;
    [Tooltip("슬로우 예고 장판 추적시간")][SerializeField] float _WarinningAreaTime;
    [Tooltip("슬로우 예고 장판 제거 지연시간")][SerializeField] float _WarinningAreaDTime;
    [Tooltip("슬로우 장판 추적속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("슬로우 위력")][SerializeField] float _SlowPower;
    [Tooltip("슬로우 지속시간")][SerializeField] float _SlowTime;

    private ParticleSystem Slow;
    private ParticleSystem Warinning;
    private GameObject Player;
    private PlayerModel model;
    private PredictiveAim _predictiveAim;
    private bool ActivateWarinning = false;
    private bool ActivateSlow = false;

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
        if (Player != null)
        {
            model = Player.GetComponent<PlayerModel>();
        }
    }

    protected override void Update()
    {
        if (ActivateWarinning && Warinning != null)
        {
            Warinning.transform.position = Vector3.MoveTowards(
                Warinning.transform.position,
                Player.transform.position,
                _ChaseSpeed * Time.deltaTime
            );
        }

        if (ActivateSlow && Slow != null)
        {
            // 슬로우 장판 범위
            bool isHit = Physics.CheckSphere(Slow.transform.position, _SlowRange, _predictiveAim.targetLayer);
            if (isHit && model != null)
            {
                model.SlowDebuff(_SlowPower, _SlowTime);
            }
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        float targetScale = _SlowRange * 1.2f;

        _isPatternActive = true;
        ActivateWarinning = true;
        Warinning = PoolManager.Instance.Spawn(_WarinningParticle, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        Warinning.transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        Warinning.Clear();
        Warinning.Play();

        yield return new WaitForSeconds(_WarinningAreaTime);

        ActivateWarinning = false;
        Vector3 spawnPos = Warinning.transform.position;

        StartCoroutine(DelayedWarinningDespawn(Warinning, targetScale));
        Warinning = null;

        ActivateSlow = true;
        Slow = PoolManager.Instance.Spawn(_SlowParticle, spawnPos, Quaternion.identity);
        Slow.transform.localScale = new Vector3(targetScale, targetScale, targetScale);

        PlayPatternSound(PatternEnum.SlowArea);
        Slow.Clear();
        Slow.Play();

        yield return new WaitForSeconds(_SlowAreaTime);

        ActivateSlow = false;
        if (Slow != null)
        {
            Slow.Stop();
            PoolManager.Instance.Despawn(Slow.gameObject);
            Slow = null;
        }

        _isPatternActive = false;
    }

    private IEnumerator DelayedWarinningDespawn(ParticleSystem target, float scale)
    {
        yield return new WaitForSeconds(_WarinningAreaDTime);
        if (target != null)
        {
            target.Stop();
            PoolManager.Instance.Despawn(target.gameObject);
        }
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
        ActivateWarinning = false;
        ActivateSlow = false;

        if (Warinning != null)
        {
            Warinning.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(Warinning.gameObject);
            Warinning = null;
        }

        if (Slow != null)
        {
            Slow.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(Slow.gameObject);
            Slow = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Slow != null)
            Gizmos.DrawWireSphere(Slow.transform.position, _SlowRange);
        else if (Warinning != null)
            Gizmos.DrawWireSphere(Warinning.transform.position, _SlowRange);
        else
            Gizmos.DrawWireSphere(transform.position, _SlowRange);
    }
}