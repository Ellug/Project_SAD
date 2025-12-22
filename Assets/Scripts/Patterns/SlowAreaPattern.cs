using Unity.VisualScripting;
using UnityEngine;

public class SlowAreaPattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
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
    [Tooltip("플레이어 이동경로 예상 여부")] public bool _PredictiveAimOn = true;
    private ParticleSystem Slow;
    private ParticleSystem Warinning;
    public GameObject Player;
    private PlayerModel model;
    private bool isHit;
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
            isHit = Physics.CheckSphere(Slow.transform.position, _SlowRange, _predictiveAim.targetLayer);
            if (isHit)
            {
                model.SlowDebuff(_SlowPower, _SlowTime);
            }
        }
    }

    public void SetWarinning() 
    {
        ActivateWarinning = true;
        Warinning = Instantiate(_WarinningParticle);
        Warinning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        Warinning.Play();
        Invoke("DestoryWarinning", _WarinningAreaTime);
    }

    public void SetSlow() 
    {
        ActivateSlow = true;
        Slow = Instantiate(_SlowParticle);
        Slow.transform.position = Warinning.transform.position;
        Slow.Play();
        Invoke("DestorySlow", _SlowAreaTime);
    }

    protected void DestoryWarinning() 
    {
        ActivateWarinning = false;
        Destroy(Warinning, _WarinningAreaDTime);
        SetSlow();
    }

    protected void DestorySlow()
    {
        ActivateSlow = false;
        Destroy(Slow);
    }

    protected override void PatternLogic()
    {
        SetWarinning();
    }

    public override void Init(GameObject target)
    {
        Player = target;
    }


    //범위 태스트용 기즈모
    private void OnDrawGizmos()
    {
        if (Slow == null) return;

        // 실제 로직과 동일한 체크 수행 (디버그용)
        bool isHit = Physics.CheckSphere(Slow.transform.position, _SlowRange, _predictiveAim.targetLayer);

        // 감지되면 초록색, 아니면 빨간색
        Gizmos.color = isHit ? Color.green : Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(Slow.transform.position, _SlowRange);
    }
}
