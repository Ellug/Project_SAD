using UnityEngine;

public class PlayerBullet : BulletBase
{
    private bool _counterAttack;
    private float _bulletSpeed;
    private float _attackPower;
    private float _bulletRange;

    private Transform _createdPos;
    private Transform _startPos;

    public float AttackPower => _attackPower;

    // TODO : 총알이 가져야할 정보는 무기로부터 오며, 이 Init 메서드를 통해서 할당
    public void Init(WeaponData weaponData, Transform weaponPos)
    {
        _startPos = weaponPos;
        _createdPos = transform;
        _attackPower = weaponData.attack;
        _bulletSpeed = weaponData.projectileSpeed;
        _bulletRange = weaponData.projectileRange;
    }

    void Update()
    {
        // 전진하다가 이동한 거리가 bulletRange를 넘어서면 파괴.
        transform.Translate(_bulletSpeed * Time.deltaTime * Vector3.forward);
        if (Vector3.Distance(_createdPos.position, _startPos.position) > _bulletRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Boss"))
        {
            if (other.TryGetComponent<BossController>(out var boss))
                boss.TakeDamage(_attackPower, _counterAttack);

            Destroy(gameObject);
        }
    }
}
