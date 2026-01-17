using System.Collections;
using UnityEngine;

public class SlowAreaPattern : PatternBase
{
    [Tooltip("슬로우 파티클")][SerializeField] ParticleSystem _SlowParticle;
    [Tooltip("슬로우 장판 범위")][SerializeField] float _SlowRange;
    [Tooltip("슬로우 장판 지속시간")][SerializeField] float _SlowAreaTime;
    [Tooltip("슬로우 장판 추적속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("슬로우 위력")][SerializeField] float _SlowPower;
    [Tooltip("슬로우 지속시간")][SerializeField] float _SlowTime;

    [Tooltip("슬로우 호출 간격")][SerializeField] private float _applyInterval = 0.2f;
    private float _nextApplyTime = 0f;

    // 슬로우 노드 2개
    private readonly StatMod[] _slowMods = new StatMod[2];

    private ParticleSystem Slow;
    private PlayerModel model;
    private bool ActivateSlow = false;

    public override void Init(GameObject target)
    {
        base.Init(target);
        if (_target != null)
        {
            model = _target.GetComponent<PlayerModel>();
        }
    }

    protected override void Update()
    {
        base.Update();

        if (ActivateSlow && Slow != null)
        {
            // 슬로우 장판 범위
            bool isHit = Physics.CheckSphere(Slow.transform.position, _SlowRange, _predictiveAim.targetLayer);

            if (isHit && model != null)
            {
                if (Time.time >= _nextApplyTime)
                {
                    model.ApplyDebuff(_slowMods, _SlowTime, _SlowTime);
                    _nextApplyTime = Time.time + _applyInterval;
                }
            }
            else
            {
                _nextApplyTime = 0f;
            }
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        float targetScale = _SlowRange * 1.2f;

        yield return StartCoroutine(ShowWarning());

        Vector3 spawnPos = _warningTransform != null ? _warningTransform.position : transform.position;

        RemoveWarning();

        ActivateSlow = true;
        // slow 데이터 처리
        _slowMods[0] = new StatMod { stat = StatId.Player_MaxSpeed, op = ModOp.Mul, value = -_SlowPower };
        _slowMods[1] = new StatMod { stat = StatId.Player_AccelForce, op = ModOp.Mul, value = -_SlowPower };

        Slow = PoolManager.Instance.Spawn(_SlowParticle, spawnPos, Quaternion.identity);
        Slow.transform.localScale = new Vector3(targetScale, targetScale, targetScale);

        PlayPatternSound(PatternEnum.SlowArea);
        Slow.Clear();
        Slow.Play();

        yield return new WaitForSeconds(_SlowAreaTime);

        CleanupPattern();
    }

    protected override void CleanupPattern()
    {
        ActivateSlow = false;
        if (Slow != null)
        {
            Slow.Stop();
            if (PoolManager.Instance != null) PoolManager.Instance.Despawn(Slow.gameObject);
            Slow = null;
        }
        RemoveWarning();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Slow != null)
            Gizmos.DrawWireSphere(Slow.transform.position, _SlowRange);
        else if (_warningTransform != null)
            Gizmos.DrawWireSphere(_warningTransform.position, _SlowRange);
    }
}