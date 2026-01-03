using System.Collections;
using UnityEngine;

public class GuidedMissilePattern : PatternBase
{
    [Header("패턴 설정")]
    [SerializeField, Tooltip("미사일 프리팹")] private GuidedMissile _MissilePrefab;
    [SerializeField, Tooltip("미사일 생성 위치")] private GameObject _SpawnPoint;

    [Header("미사일 세부 설정")]
    [SerializeField, Tooltip("미사일 이동 속도")] private float _MissileSpeed = 15f;
    [SerializeField, Tooltip("미사일 회전 속도")] private float _MissileRotationSpeed = 3f;
    [SerializeField, Tooltip("미사일 유지 시간")] private float _MissileLifeTime = 5f;
    [SerializeField, Tooltip("직격 데미지")] private float _dmg = 20f;

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        MissileLaunch();

        RemoveWarning();
    }

    private void MissileLaunch()
    {
        if (_MissilePrefab == null || _SpawnPoint == null) return;

        PlayPatternSound(PatternEnum.GuidedMissile);

        Quaternion launchRotation;
        if (_useFixedSpawnPoint)
        {
            launchRotation = _SpawnPoint.transform.rotation;
        }
        else
        {
            Vector3 dir = (_target.transform.position - _SpawnPoint.transform.position).normalized;
            launchRotation = dir != Vector3.zero ? Quaternion.LookRotation(dir) : _SpawnPoint.transform.rotation;
        }

        GuidedMissile missile = PoolManager.Instance.Spawn(_MissilePrefab, _SpawnPoint.transform.position, launchRotation);

        if (missile != null)
        {
            missile.Init(_target, _MissileSpeed, _MissileRotationSpeed, _MissileLifeTime, _dmg);
        }
    }

    protected override void CleanupPattern()
    {
    }
}