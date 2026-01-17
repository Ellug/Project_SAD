using System.Collections;
using UnityEngine;

public class FireBallPattern : PatternBase
{
    [Header("발사 설정")]
    [SerializeField, Tooltip("화염구 프리팹")] private FireBall _FireBallPrefab;
    [SerializeField, Tooltip("화염구 생성 위치")] private GameObject _SpawnPoint;
    [SerializeField, Tooltip("화염구 생성 Y좌표 오프셋")] private float _SpawnYOffset = 5f;

    [Header("투사체 및 장판 수치 설정")]
    [SerializeField, Tooltip("투사체 데미지")] private float _fireballDmg = 15f;
    [SerializeField, Tooltip("투사체 속도")] private float _fireballSpeed = 15f;
    [SerializeField, Tooltip("장판 데미지")] private float _areaDmg = 10f;
    [SerializeField, Tooltip("장판 지름")] private float _areaRange = 5f;
    [SerializeField, Tooltip("장판 지속 시간")] private float _areaLifeTime = 5f;

    [Header("화상 디버프 설정")]
    [SerializeField, Tooltip("화상 지속 시간")] private float _burnTime = 2.0f;
    [SerializeField, Tooltip("화상 틱 데미지")] private float _burnDmg = 5.0f;
    [SerializeField, Tooltip("화상 데미지 간격(틱)")] private float _burnTickInterval = 0.5f;

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Vector3 targetPosition;
        if (_useFixedSpawnPoint)
        {
            targetPosition = transform.position + (transform.forward * _WarnningMaxLength);
        }
        else
        {
            targetPosition = _warningTransform != null ? _warningTransform.position : _target.transform.position;
        }

        RemoveWarning();

        Fire(targetPosition);
        PlayPatternSound(PatternEnum.FireBall);
    }

    private void Fire(Vector3 targetPosition)
    {
        if (_FireBallPrefab == null || _SpawnPoint == null) return;

        Vector3 firePos = _SpawnPoint.transform.position;
        firePos.y += _SpawnYOffset;

        FireBall fireball = PoolManager.Instance.Spawn(_FireBallPrefab, firePos, Quaternion.identity);
        if (fireball != null)
        {
            fireball.Init(_fireballDmg, _fireballSpeed, 0);
            fireball.SetFireBallStats(_target, targetPosition, _areaDmg, _areaRange, _areaLifeTime, _burnDmg, _burnTime, _burnTickInterval);
        }
    }

    protected override void CleanupPattern() { }
}