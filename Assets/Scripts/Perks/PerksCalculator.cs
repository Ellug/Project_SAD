using UnityEngine;
using System.Collections.Generic;

public static class PerkCalculator
{
    public static void ApplyToPlayer(ref PlayerRuntimeStats s, IEnumerable<StatMod> mods)
    {
        if (mods == null) return;

        foreach (var m in mods)
        {
            switch (m.stat)
            {
                case StatId.Player_MaxHp: ClacFloat(ref s.MaxHp, m); break;
                case StatId.Player_MaxSpeed: ClacFloat(ref s.MaxSpeed, m); break;
                case StatId.Player_AccelForce: ClacFloat(ref s.AccelForce, m); break;
                case StatId.Player_RotSpeed: ClacFloat(ref s.RotSpeed, m); break;
                case StatId.Player_DodgeDuration: ClacFloat(ref s.DodgeDuration, m); break;
                case StatId.Player_DodgeSpeed: ClacFloat(ref s.DodgeSpeed, m); break;
                case StatId.Player_DodgeCoolTime: ClacFloat(ref s.DodgeCoolTime, m); break;
                case StatId.Player_SpecialCoolTime: ClacFloat(ref s.SpecialCoolTime, m); break;
                case StatId.Player_AttackSlowRate: ClacFloat(ref s.AttackSlowRate, m); break;
                case StatId.Player_AttackSlowDuration: ClacFloat(ref s.AttackSlowDuration, m); break;
            }
        }
    }

    public static void ApplyToWeapon(ref WeaponRuntimeStats s, IEnumerable<StatMod> mods)
    {
        if (mods == null) return;

        foreach (var m in mods)
        {
            switch (m.stat)
            {
                case StatId.Weapon_Attack: ClacFloat(ref s.Attack, m); break;
                case StatId.Weapon_AttackSpeed: ClacFloat(ref s.AttackSpeed, m); break;
                case StatId.Weapon_ProjectileCount: CalcInt(ref s.ProjectileCount, m); break;
                case StatId.Weapon_ProjectileRange: ClacFloat(ref s.ProjectileRange, m); break;
                case StatId.Weapon_ProjectileSpeed: ClacFloat(ref s.ProjectileSpeed, m); break;

                case StatId.Weapon_SpecialAttack: ClacFloat(ref s.SpecialAttack, m); break;
                case StatId.Weapon_SpecialBeforeDelay: ClacFloat(ref s.SpecialAttackBeforeDelay, m); break;
                case StatId.Weapon_SpecialAfterDelay: ClacFloat(ref s.SpecialAttackAfterDelay, m); break;
                case StatId.Weapon_SpecialProjectileCount: CalcInt(ref s.SpecialProjectileCount, m); break;
                case StatId.Weapon_SpecialProjectileRange: ClacFloat(ref s.SpecialProjectileRange, m); break;
                case StatId.Weapon_SpecialProjectileSpeed: ClacFloat(ref s.SpecialProjectileSpeed, m); break;
            }
        }
    }

    private static void ClacFloat(ref float target, StatMod mod)
    {
        switch (mod.op)
        {
            case ModOp.Add: target += mod.value; break;
            case ModOp.Mul: target *= (1f + mod.value); break;
            case ModOp.Override: target = mod.value; break;
        }
    }

    private static void CalcInt(ref int target, StatMod mod)
    {
        switch (mod.op)
        {
            case ModOp.Add: target += Mathf.RoundToInt(mod.value); break;
            case ModOp.Mul: target = Mathf.RoundToInt(target * (1f + mod.value)); break;
            case ModOp.Override: target = Mathf.RoundToInt(mod.value); break;
        }
    }
}
