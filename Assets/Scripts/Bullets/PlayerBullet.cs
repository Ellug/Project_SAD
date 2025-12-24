using UnityEngine;

public enum BulletEffect
{
    None,
    Burn
}

public struct BulletEffectPayload
{
    public BulletEffect effect;
    public float burnDps;
    public float burnDuration;
}

public class PlayerBullet : BulletBase
{
    private bool _counterAttack;
    private BulletEffectPayload _payload;

    public void Init(WeaponRuntimeStats stats, bool counterAttack = false, BulletEffectPayload payload = default)
    {
        _counterAttack = counterAttack;
         _payload = payload;

        base.Init(
            dmg: stats.Attack,
            speed: stats.ProjectileSpeed,
            maxDistance: stats.ProjectileRange
        );
    }

    public override void OnDespawned()
    {
        _counterAttack = false;
        _payload = default;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Boss"))
        {
            if (other.TryGetComponent<BossController>(out var boss))
            {
                boss.TakeDamage(_dmg, _counterAttack);

                if (_payload.effect == BulletEffect.Burn)
                    boss.ApplyBurn(_payload.burnDps, _payload.burnDuration);                
            }
        }
        Despawn();
    }
}
