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

    private GameObject _target;
    private WaitForSeconds _delay;

    protected override void Awake()
    {
        base.Awake();
        _delay = new WaitForSeconds(_shootInterval);
    }

    public override void Init(GameObject target)
    {
        _target = target;
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        for (int i = 0; i < _shootBulletNumber; i++)
        {
            // 총알이 수평으로 날아가게 하기 위해 y축은 타겟 좌표가 아님
            if (_target == null) yield break;

            Vector3 spawnPos = _spawnPosition != null ? _spawnPosition.position : transform.position;

            Vector3 targetPos = new Vector3(
                _target.transform.position.x,
                spawnPos.y,
                _target.transform.position.z
            );

            Vector3 dir = targetPos - spawnPos;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                PlayPatternSound(PatternEnum.NormalShot);

                BulletBase bullet = PoolManager.Instance.Spawn(_bulletPrefab, spawnPos, rot);
                if (bullet != null)
                {
                    bullet.Init(_bulletDamage, _bulletSpeed, _bulletDistance);
                }
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