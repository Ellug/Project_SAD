using UnityEngine;

public class PlayerBullet : BulletBase
{
    private bool _counterAttack;

    public void Init(WeaponData weaponData, bool counterAttack = false)
    {
        _counterAttack = counterAttack;

        base.Init(
            dmg: weaponData.attack,
            speed: weaponData.projectileSpeed,
            maxDistance: weaponData.projectileRange
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
