using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PrecisionStrikePattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("폭발 파티클")][SerializeField] ParticleSystem _ExplosionParticle;
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("폭발 지연 시간")][SerializeField] float _ExpolsionTime;
    [Tooltip("폭발 판정 범위")][SerializeField] float _ExplosionRange;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("경고 장판 지연시간")][SerializeField] float _WarnningDTime;
    [Tooltip("데미지")][SerializeField] float _Dmg;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    private ParticleSystem _Warnning;
    private ParticleSystem _Explosion;    
    private bool chase = false;

    void Update()
    {
        if (chase && _Warnning != null)
        {
            _Warnning.transform.position = Vector3.MoveTowards(
                _Warnning.transform.position,
                 Player.transform.position,
                _ChaseSpeed * Time.deltaTime
            );
        }
    }

    public void WarnningEffect() 
    {        
        _Warnning = Instantiate(_WarnningArea);
        _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        chase = true;
        StartCoroutine(Chase());
        _Warnning.Play();
    }

    private void PrecisionStrike() 
    {
        chase = false;
        StartCoroutine(Explosion());
        Destroy(_Warnning.gameObject, _WarnningDTime);
    }

    private IEnumerator Chase() 
    {
        yield return new WaitForSeconds(_WarnningTime);
        PrecisionStrike();
    }

    private IEnumerator Explosion()
    {
        yield return new WaitForSeconds(_ExpolsionTime);
        _Explosion = Instantiate(_ExplosionParticle, _Warnning.transform.position, _Warnning.transform.rotation);
        var main = _Explosion.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        _Explosion.Play();
        bool isHit = Physics.CheckSphere(_Explosion.transform.position, _ExplosionRange, _predictiveAim.targetLayer);
        if (isHit)
        {
            Player.TryGetComponent<PlayerModel>(out var player);
            player.TakeDamage(_Dmg);
        }
    }

    protected override void PatternLogic()
    {
        WarnningEffect();
    }

    public override void Init(GameObject target)
    {
        Player = target;
    }
}
