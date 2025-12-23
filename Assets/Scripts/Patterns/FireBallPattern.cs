using UnityEngine;
using System.Collections;

public class FireBallPattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("플레이어 예측 거리 (ex:0이면 플레이어 위치)")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("경고 장판 지연시간")][SerializeField] float _WarnningDTime;
    [Tooltip("화염구 프리팹")][SerializeField] FireBall _FireBallPrefab;
    [Tooltip("화염구 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    private ParticleSystem _Warnning;
    private bool chase = false;
    private Transform WarnningPoint;
    private Coroutine WarnningDelayCoroutine;

    private void Update()
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
    private IEnumerator Chase()
    {
        yield return new WaitForSeconds(_WarnningTime);
        WarnningPoint = _Warnning.transform;
        Destroy(_Warnning.gameObject, _WarnningDTime);
        Fire();
    }

    private void Fire()
    {
        StopCoroutine(WarnningDelayCoroutine);
        FireBall fireball = PoolManager.Instance.Spawn(_FireBallPrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
        fireball.setTarget(WarnningPoint);
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

