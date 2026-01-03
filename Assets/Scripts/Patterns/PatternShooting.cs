using System.Collections;
using UnityEngine;

public class PatternShooting : PatternBase
{
    [Header("투사체 패턴 속성")]
    [SerializeField, Tooltip("사용할 총알 프리팹")] private BulletBase _bulletPrefab;
    [SerializeField, Tooltip("총알 발사 간격")] private float _shootInterval = 0.1f;
    [SerializeField, Tooltip("발사할 총알 개수")] private int _shootBulletNumber;
    [SerializeField, Tooltip("총알 스폰 위치")] private Transform _spawnPosition;
    [SerializeField, Tooltip("총알 데미지")] private float _bulletDamage;
    [SerializeField, Tooltip("총알 속도")] private float _bulletSpeed;
    [SerializeField, Tooltip("총알 최대 사거리")] private float _bulletDistance;

    private WaitForSeconds _delay;

    protected override void Awake()
    {
        base.Awake();
        _delay = new WaitForSeconds(_shootInterval);
    }

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        for (int i = 0; i < _shootBulletNumber; i++)
        {
            if (_target == null) yield break;

            Vector3 spawnPos = _spawnPosition != null ? _spawnPosition.position : transform.position;
            Quaternion shootRot;

            if (_useFixedSpawnPoint)
            {
                // 고정 스폰 포인트의 방향을 그대로 사용
                shootRot = _spawnPosition != null ? _spawnPosition.rotation : transform.rotation;
            }
            else
            {
                // 플레이어 방향 계산 (수평 발사를 위해 Y값 보정)
                Vector3 targetPos = new Vector3(_target.transform.position.x, spawnPos.y, _target.transform.position.z);
                Vector3 dir = (targetPos - spawnPos).normalized;

                if (dir == Vector3.zero) dir = transform.forward;
                shootRot = Quaternion.LookRotation(dir);
            }

            PlayPatternSound(PatternEnum.NormalShot);

            BulletBase bullet = PoolManager.Instance.Spawn(_bulletPrefab, spawnPos, shootRot);
            if (bullet != null)
            {
                bullet.Init(_bulletDamage, _bulletSpeed, _bulletDistance);
            }

            yield return _delay;
        }

        _isPatternActive = false;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
    }
}