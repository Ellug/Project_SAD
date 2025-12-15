using System.Collections;
using UnityEngine;

public class PatternShooting : PatternBase
{
    [Header("투사체 패턴 속성")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _shootInterval = 0.1f;
    [SerializeField] private int _shootBulletNumber;
    [SerializeField] private Transform _spawnPosition;

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
        transform.position = _spawnPosition.position;
    }

    protected override void PatternLogic()
    {
        StartCoroutine(ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        for (int i = 0; i < _shootBulletNumber; i++)
        {
            // 총알이 수평으로 날아가게 하기 위해 y축은 타겟 좌표가 아님
            Vector3 target = new Vector3(
                _target.transform.position.x,
                transform.position.y,
                _target.transform.position.z
                );

            // 생성하고 타겟 방향으로 돌린다.
            Instantiate(_bulletPrefab, transform.position, transform.rotation)
                .transform.LookAt(target);

            yield return _delay;
        }
    }

    
}
