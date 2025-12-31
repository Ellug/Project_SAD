using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireBallPattern : PatternBase
{
    [Header("추적 및 예측")]
    [SerializeField, Tooltip("경고 장판 프리팹")] private ParticleSystem _WarnningArea;
    [SerializeField, Tooltip("플레이어 예측 거리")] private float _ChaseOffset;
    [SerializeField, Tooltip("경고 파티클 추적 시간")] private float _WarnningTime;
    [SerializeField, Tooltip("경고 장판 지연시간")] private float _WarnningDTime;

    [Header("발사 설정")]
    [SerializeField, Tooltip("화염구 프리팹")] private FireBall _FireBallPrefab;
    [SerializeField, Tooltip("화염구 생성 위치")] private GameObject _SpawnPoint;
    [SerializeField, Tooltip("화염구 생성 Y좌표 오프셋")] private float _SpawnYOffset = 5f;

    private GameObject _player;
    private ParticleSystem _currentWarning;
    private PredictiveAim _predictiveAim;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void Update()
    {
        if (_isPatternActive && _currentWarning != null && _player != null)
        {
            _currentWarning.transform.position = _predictiveAim.PredictiveAimCalc(_ChaseOffset);
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        if (_WarnningArea != null && _predictiveAim != null)
        {
            _currentWarning = PoolManager.Instance.Spawn(_WarnningArea, _predictiveAim.PredictiveAimCalc(_ChaseOffset), Quaternion.identity);
            _isPatternActive = true;

            _currentWarning.Clear();
            _currentWarning.Play();
        }

        yield return new WaitForSeconds(_WarnningTime);
        _isPatternActive = false;

        Vector3 targetPosition = _currentWarning.transform.position;

        yield return new WaitForSeconds(_WarnningDTime);

        Fire(targetPosition);
        PlayPatternSound(PatternEnum.FireBall);

        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
        }
    }

    private void Fire(Vector3 targetPosition)
    {
        if (_FireBallPrefab == null || _SpawnPoint == null) return;

        Vector3 firePos = _SpawnPoint.transform.position;
        firePos.y += _SpawnYOffset;

        FireBall fireball = PoolManager.Instance.Spawn(_FireBallPrefab, firePos, _SpawnPoint.transform.rotation);
        if (fireball != null)
        {
            fireball.Init(_player);
            fireball.setTarget(targetPosition);
        }
    }

    protected override void CleanupPattern()
    {
        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
        }
    }
}
