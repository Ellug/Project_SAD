using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class IceAreaPattern : PatternBase
{
    [Header("경고 장판")]
    [Tooltip("경고 파티클")][SerializeField] private ParticleSystem _WarnningArea;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] private float _WarnningTime = 2.0f;
    [Tooltip("경고 장판 정지 후 대기 시간")][SerializeField] private float _WarnningDTime = 0.5f;

    [Header("냉기 장판 생성")]
    [Tooltip("냉기 장판 프리팹 (IceArea 컴포넌트 포함)")][SerializeField] private IceArea _IceAreaPrefab;
    [Tooltip("생성 위치 예측 오프셋")][SerializeField] private float _ChaseOffset = 0.2f;

    private PredictiveAim _predictiveAim;
    private GameObject Player;
    private ParticleSystem _Warnning;
    private bool chase = false;

    public override void Init(GameObject target)
    {
        Player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void PatternLogic()
    {
        WarnningEffect();
    }

    void Update()
    {
        if (chase && _Warnning != null && _predictiveAim != null)
        {
            _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    public void WarnningEffect()
    {
        if (_WarnningArea == null) return;

        _Warnning = PoolManager.Instance.Spawn(_WarnningArea, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
        _Warnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        chase = true;

        _Warnning.Clear();
        _Warnning.Play();
        StartCoroutine(ChaseRoutine());
    }

    private IEnumerator ChaseRoutine()
    {
        yield return new WaitForSeconds(_WarnningTime);

        chase = false;
        Vector3 stopPos = transform.position;

        if (_Warnning != null)
        {
            stopPos = _Warnning.transform.position;
        }

        yield return new WaitForSeconds(_WarnningDTime);

        if (_Warnning != null)
        {
            _Warnning.Stop();
            PoolManager.Instance.Despawn(_Warnning.gameObject);
            _Warnning = null;
        }

        SpawnIceArea(stopPos);
    }

    private void SpawnIceArea(Vector3 position)
    {
        if (_IceAreaPrefab == null) return;

        PlayPatternSound(PatternEnum.IceArea);

        IceArea ice = PoolManager.Instance.Spawn(_IceAreaPrefab, position, Quaternion.identity);

        if (ice != null)
        {
            ice.Init(Player);
        }
    }
}