using UnityEngine;
using System.Collections;

public class GuidedMissilePattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("경고 장판 지연시간")][SerializeField] float _WarnningDTime;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("미사일 프리팹")][SerializeField] private GuidedMissile _MissilePrefab;
    [Tooltip("미사일 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    private ParticleSystem _Warnning;
    private bool chase = false;
    private Coroutine WarnningDelayCoroutine;

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
        MissileLaunch();
    }

    private void MissileLaunch() 
    {
        StopCoroutine(WarnningDelayCoroutine);
        GuidedMissile missile = PoolManager.Instance.Spawn(_MissilePrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
    }

    public override void Init(GameObject target) 
    { 
    }
    protected override void PatternLogic() 
    {
    }
}
