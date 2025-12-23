using UnityEngine;
using System.Collections;

public class FireCannonPattern : PatternBase
{
    [Tooltip("PredictiveAim")][SerializeField] PredictiveAim _predictiveAim;
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("경고 장판 지연시간")][SerializeField] float _WarnningDTime;
    [Tooltip("화염포 프리팹")][SerializeField] FireCannon _FireCannonPrefab;
    [Tooltip("화염포 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
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
        Fire();
    }

    private void Fire()
    {
        StopCoroutine(WarnningDelayCoroutine);
        FireCannon fireCannon = PoolManager.Instance.Spawn(_FireCannonPrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
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
