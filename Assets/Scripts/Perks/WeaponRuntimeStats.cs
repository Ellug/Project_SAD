public struct WeaponRuntimeStats
{
    // Normal
    public float Attack;
    public float AttackSpeed;
    public int ProjectileCount;
    public float ProjectileAngle;
    public float ProjectileRange;
    public float ProjectileSpeed;
    public PlayerBullet ProjectilePrefab;

    // Special
    public float SpecialAttack;
    public float SpecialAttackBeforeDelay;
    public float SpecialAttackAfterDelay;
    public int SpecialProjectileCount;
    public float SpecialProjectileAngle;
    public float SpecialProjectileRange;
    public float SpecialProjectileSpeed;
    public PlayerBullet SpecialProjectilePrefab;

    // Dynamic Mode
    public int RifleMode; // 0 none 1 Nobrain 2 Minigun
    public int ShotgunMode; // 0 none 1 Slug 2 Triple
    public int SniperMode; // 0 none 1 Bouncing 2 CurtainCall

    public static WeaponRuntimeStats FromData(WeaponData d)
    {
        return new WeaponRuntimeStats
        {
            Attack = d.attack,
            AttackSpeed = d.attackSpeed,
            ProjectileCount = d.projectileCount,
            ProjectileAngle = d.projectileAngle,
            ProjectileRange = d.projectileRange,
            ProjectileSpeed = d.projectileSpeed,
            ProjectilePrefab = d.projectilePrefab,

            SpecialAttack = d.SpecialAttack,
            SpecialAttackBeforeDelay = d.SpecialAttackBeforeDelay,
            SpecialAttackAfterDelay = d.SpecialAttackAfterDelay,
            SpecialProjectileCount = d.SpecialProjectileCount,
            SpecialProjectileAngle = d.SpecialProjectileAngle,
            SpecialProjectileRange = d.SpecialProjectileRange,
            SpecialProjectileSpeed = d.SpecialProjectileSpeed,
            SpecialProjectilePrefab = d.SpecialProjectilePrefab,

            RifleMode = 0,
            ShotgunMode = 0,
            SniperMode = 0,
        };
    }
}
