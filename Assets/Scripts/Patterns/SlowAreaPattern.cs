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

    private void Start()
    {
        model = Player.GetComponent<PlayerModel>();
    }

    private void Update()
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
            bool isHit = Physics.CheckSphere(Slow.transform.position, _SlowRange, _predictiveAim.targetLayer);
            if (isHit)
            {
                model.SlowDebuff(_SlowPower, _SlowTime);
            }
        }
    }

    private IEnumerator SlowSequence()
    {
        float targetScale = _SlowRange * 1.2f;

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

    protected override void PatternLogic()
    {
        StartCoroutine(SlowSequence());
    }

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
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