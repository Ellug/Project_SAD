using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GuidedMissilePattern : PatternBase
{
    
    [Tooltip("경고 파티클")][SerializeField] ParticleSystem _WarnningArea;
    [Tooltip("생성 위치 거리")][SerializeField] float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] float _WarnningTime;
    [Tooltip("장판 추적 속도")][SerializeField] float _ChaseSpeed;
    [Tooltip("미사일 프리팹")][SerializeField] private GuidedMissile _MissilePrefab;
    [Tooltip("미사일 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    private ParticleSystem _Warnning;
    private GameObject Player;
    private bool chase = false;
    private Coroutine ChaseCoroutine;
    private PredictiveAim _predictiveAim;


    void Update()
    {
        if (chase && _Warnning != null)
            _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
    }

    public void WarnningEffect()
    {
        _Warnning = PoolManager.Instance.Spawn(_WarnningArea, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        chase = true;
        ChaseCoroutine = StartCoroutine(Chase());
        _Warnning.Clear();
        _Warnning.Play();
    }

    private IEnumerator Chase()
    {
        yield return new WaitForSeconds(_WarnningTime);

        if (_Warnning != null)
        {
            _Warnning.Stop();
            PoolManager.Instance.Despawn(_Warnning.gameObject);
            _Warnning = null;
        }
        MissileLaunch();
    }

    private void MissileLaunch() 
    {
        StopCoroutine(ChaseCoroutine);
        GuidedMissile missile = PoolManager.Instance.Spawn(_MissilePrefab, _SpawnPoint.transform.position, _SpawnPoint.transform.rotation);
        PlayPatternSound(PatternEnum.GuidedMissile);
    }

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void PatternLogic() 
    {
        WarnningEffect();
    }
}
