using UnityEngine;
using System.Collections;

public class FireBallPattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("경고 장판 지연시간")][SerializeField] float _WarnningDTime;
    [Tooltip("파이어볼 프리팹")][SerializeField] FireBall _FireBallPrefab;
    [Tooltip("파이어볼 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    private ParticleSystem _Warnning;
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

    private IEnumerator Chase()
    {
        yield return new WaitForSeconds(_WarnningTime);
        Destroy(_Warnning.gameObject, _WarnningDTime);
        Fire();
    }

    private void Fire()
    {
        FireBall fireBall = PoolManager.Instance.Spawn(_FireBallPrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
    }
    protected override void PatternLogic()
    {
    }

    public override void Init(GameObject target)
    {
    }
}
