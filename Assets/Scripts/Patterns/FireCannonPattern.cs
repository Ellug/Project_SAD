using System.Collections;
using UnityEngine;

public class FireCannonPattern : PatternBase
{
    [Header("화염포 설정")]
    [SerializeField, Tooltip("화염포 프리팹")] private FireCannon _FireCannonPrefab;
    [SerializeField, Tooltip("화염포 생성 위치")] private Transform _SpawnPoint;

    [Header("투사체 수치 설정")]
    [SerializeField, Tooltip("투사체 데미지")] private float _cannonDmg = 20f;
    [SerializeField, Tooltip("투사체 속도")] private float _cannonSpeed = 20f;
    [SerializeField, Tooltip("투사체 사거리")] private float _cannonRange = 30f;

    [Header("화상 디버프 설정")]
    [SerializeField, Tooltip("화상 지속시간")] private float _burnTime = 2.0f;
    [SerializeField, Tooltip("화상 데미지")] private float _burnDmg = 5.0f;
    [SerializeField, Tooltip("화상 틱 인터벌")] private float _burnTickInterval = 0.1f;

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Fire();

        RemoveWarning();
    }

    private void Fire()
    {
        if (_FireCannonPrefab == null || _SpawnPoint == null) return;

        PlayPatternSound(PatternEnum.FireCannon);

        Quaternion fireRotation;
        if (_useFixedSpawnPoint)
        {
            fireRotation = _SpawnPoint.rotation;
        }
        else
        {
            fireRotation = _lastDirection != Vector3.zero ? Quaternion.LookRotation(_lastDirection) : _SpawnPoint.rotation;
        }

        FireCannon instance = PoolManager.Instance.Spawn(_FireCannonPrefab, _SpawnPoint.position, fireRotation);

        if (instance != null)
        {
            instance.Init(_cannonDmg, _cannonSpeed, _cannonRange);
            instance.SetBurnStats(_burnDmg, _burnTime, _burnTickInterval);
        }
    }

    protected override void CleanupPattern()
    {
    }
}