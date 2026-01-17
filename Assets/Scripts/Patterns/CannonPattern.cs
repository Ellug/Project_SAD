using System.Collections;
using UnityEngine;

public class CannonPattern : PatternBase
{
    [Header("투사체 패턴 속성")]
    [SerializeField, Tooltip("발사할 총알 프리팹")] private BulletBase _bulletPrefab;
    [SerializeField, Tooltip("총알 발사 간격")] private float _shootInterval = 0.1f;
    [SerializeField, Tooltip("발사할 총알 수")] private int _shootBulletNumber;
    [SerializeField, Tooltip("총알 생성 위치")] private Transform _spawnPosition;

    [Header("보스 캐논 설정")]
    [SerializeField, Tooltip("캐논 데미지")] private float _bulletDamage = 10f;
    [SerializeField, Tooltip("캐논 속도")] private float _bulletSpeed = 20f;
    [SerializeField, Tooltip("캐논 최대 사거리")] private float _bulletMaxDistance = 50f;

    private WaitForSeconds _shootDelay;

    protected override void Awake()
    {
        base.Awake();
        _shootDelay = new WaitForSeconds(_shootInterval);
    }

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Vector3 fireDir = _useFixedSpawnPoint ? _spawnPosition.forward : _lastDirection;
        fireDir.y = 0f;

        RemoveWarning();

        if (fireDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(fireDir);

            for (int i = 0; i < _shootBulletNumber; i++)
            {
                BulletBase bullet = PoolManager.Instance.Spawn(_bulletPrefab, _spawnPosition.position, rotation);

                if (bullet != null)
                {
                    bullet.Init(_bulletDamage, _bulletSpeed, _bulletMaxDistance);
                }

                yield return _shootDelay;
            }
        }

        PlayPatternSound(PatternEnum.Cannon);
    }

    protected override void CleanupPattern()
    {
    }
}