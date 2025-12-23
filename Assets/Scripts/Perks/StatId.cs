public enum StatId
{
    // Player
    Player_MaxHp,
    Player_MaxSpeed,
    Player_AccelForce,
    Player_RotSpeed,
    Player_DodgeDuration,
    Player_DodgeSpeed,
    Player_DodgeCoolTime,
    Player_SpecialCoolTime,
    Player_AttackSlowRate,
    Player_AttackSlowDuration,

    // Weapon (Normal)
    Weapon_Attack,
    Weapon_AttackSpeed,
    Weapon_ProjectileCount,
    Weapon_ProjectileRange,
    Weapon_ProjectileSpeed,

    // Weapon (Special)
    Weapon_SpecialAttack,
    Weapon_SpecialBeforeDelay,
    Weapon_SpecialAfterDelay,
    Weapon_SpecialProjectileCount,
    Weapon_SpecialProjectileRange,
    Weapon_SpecialProjectileSpeed,
}

public enum ModOp { Add, Mul, Override }

public enum PerkTrigger
{
    OnSpecialUsed,
    OnDodgeUsed,
}