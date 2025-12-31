using System.Collections.Generic;
using UnityEngine;

public static class PerkCalculator
{
    private struct FloatAcc
    {
        public float add;
        public float mulSum;              // Mul 값 합산
        public bool hasOverride;
        public float overrideValue;

        public void Add(in StatMod m)
        {
            switch (m.op)
            {
                case ModOp.Add: add += m.value; break;
                case ModOp.Mul: mulSum += m.value; break;         // 여기서 곱 합산
                case ModOp.Override:
                    hasOverride = true;
                    overrideValue = m.value;                      // 마지막 Override가 승리(순서 의존)
                    break;
            }
        }

        public void Apply(ref float target)
        {
            if (hasOverride) target = overrideValue;
            target += add;
            target *= (1f + mulSum);                              // 마지막에 한 번만 곱
        }
    }

    private struct IntAcc
    {
        public int add;
        public float mulSum;
        public bool hasOverride;
        public int overrideValue;

        public void Add(in StatMod m)
        {
            switch (m.op)
            {
                case ModOp.Add: add += Mathf.RoundToInt(m.value); break;
                case ModOp.Mul: mulSum += m.value; break;         // 합산
                case ModOp.Override:
                    hasOverride = true;
                    overrideValue = Mathf.RoundToInt(m.value);
                    break;
            }
        }

        public void Apply(ref int target)
        {
            if (hasOverride) target = overrideValue;
            target += add;
            target = Mathf.RoundToInt(target * (1f + mulSum));    // 마지막에 한 번만 곱
        }
    }

    public static void ApplyToPlayer(ref PlayerRuntimeStats s, IEnumerable<StatMod> mods)
    {
        if (mods == null) return;

        FloatAcc maxHp = default, maxSpeed = default, accel = default, rot = default;
        FloatAcc dodgeDur = default, dodgeSpeed = default, dodgeCd = default;
        FloatAcc spCd = default, atkSlowRate = default, atkMinSpeed = default;

        foreach (var m in mods)
        {
            switch (m.stat)
            {
                case StatId.Player_MaxHp: maxHp.Add(m); break;
                case StatId.Player_MaxSpeed: maxSpeed.Add(m); break;
                case StatId.Player_AccelForce: accel.Add(m); break;
                case StatId.Player_RotSpeed: rot.Add(m); break;
                case StatId.Player_DodgeDuration: dodgeDur.Add(m); break;
                case StatId.Player_DodgeSpeed: dodgeSpeed.Add(m); break;
                case StatId.Player_DodgeCoolTime: dodgeCd.Add(m); break;
                case StatId.Player_SpecialCoolTime: spCd.Add(m); break;
                case StatId.Player_AttackSlowRate: atkSlowRate.Add(m); break;
                case StatId.Player_AttackMinSpeed: atkMinSpeed.Add(m); break;
            }
        }

        maxHp.Apply(ref s.MaxHp);
        maxSpeed.Apply(ref s.MaxSpeed);
        accel.Apply(ref s.AccelForce);
        rot.Apply(ref s.RotSpeed);
        dodgeDur.Apply(ref s.DodgeDuration);
        dodgeSpeed.Apply(ref s.DodgeSpeed);
        dodgeCd.Apply(ref s.DodgeCoolTime);
        spCd.Apply(ref s.SpecialCoolTime);
        atkSlowRate.Apply(ref s.AttackSlowRate);
        atkMinSpeed.Apply(ref s.AttackMinSpeed);
    }

    public static void ApplyToWeapon(ref WeaponRuntimeStats s, IEnumerable<StatMod> mods)
    {
        if (mods == null) return;

        FloatAcc atk = default, atkSpeed = default, range = default, projSpeed = default, angle = default;
        FloatAcc spAtk = default, spBefore = default, spAfter = default, spRange = default, spProjSpeed = default;
        IntAcc projCount = default, spProjCount = default;
        IntAcc rifle = default, shotgun = default, sniper = default;

        foreach (var m in mods)
        {
            switch (m.stat)
            {
                case StatId.Weapon_Attack: atk.Add(m); break;
                case StatId.Weapon_AttackSpeed: atkSpeed.Add(m); break;
                case StatId.Weapon_ProjectileCount: projCount.Add(m); break;
                case StatId.Weapon_ProjectileRange: range.Add(m); break;
                case StatId.Weapon_ProjectileSpeed: projSpeed.Add(m); break;
                case StatId.Weapon_ProjectileAngle: angle.Add(m); break;

                case StatId.Weapon_SpecialAttack: spAtk.Add(m); break;
                case StatId.Weapon_SpecialBeforeDelay: spBefore.Add(m); break;
                case StatId.Weapon_SpecialAfterDelay: spAfter.Add(m); break;
                case StatId.Weapon_SpecialProjectileCount: spProjCount.Add(m); break;
                case StatId.Weapon_SpecialProjectileRange: spRange.Add(m); break;
                case StatId.Weapon_SpecialProjectileSpeed: spProjSpeed.Add(m); break;

                case StatId.Weapon_RifleMode: rifle.Add(m); break;
                case StatId.Weapon_ShotgunMode: shotgun.Add(m); break;
                case StatId.Weapon_SniperMode: sniper.Add(m); break;
            }
        }

        atk.Apply(ref s.Attack);
        atkSpeed.Apply(ref s.AttackSpeed);
        projCount.Apply(ref s.ProjectileCount);
        range.Apply(ref s.ProjectileRange);
        projSpeed.Apply(ref s.ProjectileSpeed);
        angle.Apply(ref s.ProjectileAngle);

        spAtk.Apply(ref s.SpecialAttack);
        spBefore.Apply(ref s.SpecialAttackBeforeDelay);
        spAfter.Apply(ref s.SpecialAttackAfterDelay);
        spProjCount.Apply(ref s.SpecialProjectileCount);
        spRange.Apply(ref s.SpecialProjectileRange);
        spProjSpeed.Apply(ref s.SpecialProjectileSpeed);

        rifle.Apply(ref s.RifleMode);
        shotgun.Apply(ref s.ShotgunMode);
        sniper.Apply(ref s.SniperMode);
    }
}
