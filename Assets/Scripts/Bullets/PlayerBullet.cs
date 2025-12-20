using UnityEngine;

public class PlayerBullet : BulletBase
{
    private bool _counterAttack;

    public void Init(WeaponRuntimeStats stats, bool counterAttack = false)
    {
        _counterAttack = counterAttack;

        base.Init(
            dmg: stats.Attack,
            speed: stats.ProjectileSpeed,
            maxDistance: stats.ProjectileRange
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Boss"))
        {
            if (other.TryGetComponent<BossController>(out var boss))
                boss.TakeDamage(_dmg, _counterAttack);

            Despawn();
        }
    }
}
