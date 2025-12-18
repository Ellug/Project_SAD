using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class PerkText
{
    private static readonly HashSet<StatId> _intStats = new()
    {
        StatId.Weapon_ProjectileCount,
        StatId.Weapon_SpecialProjectileCount,
    };

    public static string Build(StatMod[] mods)
    {
        if (mods == null || mods.Length == 0)
            return "효과 없음";

        StringBuilder sb = new(128);

        for (int i = 0; i < mods.Length; i++)
        {
            var m = mods[i];
            string statName = GetStatName(m.stat);
            string opText = FormatOp(m.stat, m.op, m.value);

            if (sb.Length > 0)
                sb.AppendLine();
            
            sb.Append(statName).Append(" ").Append(opText);
        }

        return sb.ToString();
    }

    private static string GetStatName(StatId id)
    {
        return id switch
        {
            StatId.Player_MaxHp                     => "최대 체력",
            StatId.Player_MaxSpeed                  => "이동 속도",
            StatId.Player_AccelForce                => "가속도",
            StatId.Player_RotSpeed                  => "회전 속도",
            StatId.Player_DodgeDuration             => "회피 지속 시간",
            StatId.Player_DodgeSpeed                => "회피 속도",
            StatId.Player_DodgeCoolTime             => "회피 쿨타임",
            StatId.Player_SpecialCoolTime           => "특수 공격 쿨타임",
            StatId.Player_AttackSlowRate            => "공격 시 이동속도 감소 비율",
            StatId.Player_AttackSlowDuration        => "공격 시 감속 시간",

            StatId.Weapon_Attack                    => "공격력",
            StatId.Weapon_AttackSpeed               => "공격 속도",
            StatId.Weapon_ProjectileCount           => "발사 개수",
            StatId.Weapon_ProjectileRange           => "사거리",
            StatId.Weapon_ProjectileSpeed           => "탄속",

            StatId.Weapon_SpecialAttack             => "특수 공격력",
            StatId.Weapon_SpecialBeforeDelay        => "특수 선딜",
            StatId.Weapon_SpecialAfterDelay         => "특수 후딜",
            StatId.Weapon_SpecialProjectileCount    => "특수 발사 개수",
            StatId.Weapon_SpecialProjectileRange    => "특수 사거리",
            StatId.Weapon_SpecialProjectileSpeed    => "특수 탄속",

            _ => id.ToString()
        };
    }

    private static string FormatOp(StatId stat, ModOp op, float value)
    {
        bool isInt = _intStats.Contains(stat);

        switch (op)
        {
            case ModOp.Add:
                return $"{SignValue(value, isInt)}";

            case ModOp.Mul:
                float pct = value * 100f;
                return $"{SignValue(pct, false)}%";

            case ModOp.Override:
                return $"= {FormatValue(value, isInt)}";

            default:
                return $"{value}";
        }
    }

    private static string SignValue(float v, bool isInt)
    {
        string sign = v >= 0f ? "+" : "-";
        return $"{sign}{FormatValue(Mathf.Abs(v), isInt)}";
    }

    private static string FormatValue(float v, bool isInt)
    {
        if (isInt)
            return Mathf.RoundToInt(v).ToString();

        if (Mathf.Abs(v - Mathf.Round(v)) < 0.0001f)
            return Mathf.RoundToInt(v).ToString();
            
        return v.ToString("0.##");
    }
}
