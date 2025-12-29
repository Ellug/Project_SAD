using UnityEngine;
using System.Collections;

public class FireBallPattern : PatternBase
{
    [Header("추적 및 예측")]
    [Tooltip("경고 장판 프리팹")][SerializeField] private ParticleSystem _WarnningArea;
    [Tooltip("플레이어 예측 거리")][SerializeField] private float _ChaseOffset;
    [Tooltip("경고 파티클 추적 시간")][SerializeField] private float _WarnningTime;
    [Tooltip("경고 장판 지연시간")][SerializeField] private float _WarnningDTime;

    [Header("발사 설정")]
    [Tooltip("화염구 프리팹")][SerializeField] private FireBall _FireBallPrefab;
    [Tooltip("화염구 생성 위치")][SerializeField] private GameObject _SpawnPoint;
    [Tooltip("화염구 생성 Y좌표 오프셋")][SerializeField] private float _SpawnYOffset = 5f;

    private GameObject _player;
    private ParticleSystem _currentWarnning;
    private bool _isChasing = false;
    private PredictiveAim _predictiveAim;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void PatternLogic()
    {
        WarnningEffect();
    }

    private void Update()
    {
        if (_isChasing && _currentWarnning != null && _player != null)
        {
            _currentWarnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    public void WarnningEffect()
    {
        if (_WarnningArea == null) return;

        _currentWarnning = Instantiate(_WarnningArea);
        _currentWarnning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);

        _isChasing = true;
        _currentWarnning.Play();

        StartCoroutine(ChaseRoutine());
    }

    private IEnumerator ChaseRoutine()
    {
        yield return new WaitForSeconds(_WarnningTime);

        _isChasing = false;

        GameObject targetMarker = new GameObject("FireBallTargetMarker");
        targetMarker.transform.position = _currentWarnning.transform.position;

        yield return new WaitForSeconds(_WarnningDTime);

        Fire(targetMarker.transform);

        if (_currentWarnning != null) Destroy(_currentWarnning.gameObject);
        Destroy(targetMarker, 5f);
    }

    private void Fire(Transform target)
    {
        if (_FireBallPrefab == null || _SpawnPoint == null) return;

        Vector3 firePos = _SpawnPoint.transform.position;
        firePos.y += _SpawnYOffset;

        FireBall fireball = PoolManager.Instance.Spawn(_FireBallPrefab, firePos, _SpawnPoint.transform.rotation);
        if (fireball != null)
        {
            fireball.Init(_player);
            fireball.setTarget(target);
        }
    }
}