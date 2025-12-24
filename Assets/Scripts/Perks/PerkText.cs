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
            return string.Empty;

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

        return sb.Length > 0 ? sb.ToString() : string.Empty;
    }

    // 버프 케이스 오버로드
    public static string Build(TriggeredBuff[] buffs)
    {
        if (buffs == null || buffs.Length == 0)
            return string.Empty;

        StringBuilder sb = new(192);

        for (int i = 0; i < buffs.Length; i++)
        {
            var b = buffs[i];
            if (b == null) continue;

            int startLen = sb.Length;

            sb.Append($"[{GetTriggerName(b.trigger)}] (지속 {FormatSeconds(b.duration)})");

            if (b.mods != null && b.mods.Length > 0)
            {
                for (int m = 0; m < b.mods.Length; m++)
                {
                    var mod = b.mods[m];
                    string statName = GetStatName(mod.stat);
                    string opText = FormatOp(mod.stat, mod.op, mod.value);

                    sb.AppendLine();
                    sb.Append("- ").Append(statName).Append(" ").Append(opText);
                }
            }

            if (sb.Length == startLen) continue;

            if (i < buffs.Length - 1)
                sb.AppendLine().AppendLine();
        }

        return sb.Length > 0 ? sb.ToString() : string.Empty;
    }

    private static string GetTriggerName(PerkTrigger trigger)
    {
        return trigger switch
        {
            PerkTrigger.OnSpecialUsed => "특수공격 사용시",
            PerkTrigger.OnDodgeUsed   => "회피기 사용시",
            _ => trigger.ToString()
        };
    }

    private static string FormatSeconds(float sec)
    {
        if (sec <= 0f) return "0초";
        if (Mathf.Abs(sec - Mathf.Round(sec)) < 0.0001f)
            return $"{Mathf.RoundToInt(sec)}초";
        return $"{sec:0.##}초";
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
            StatId.Player_AttackSlowRate            => "공격 시 이동속도 감소량",
            StatId.Player_AttackMinSpeed            => "공격 이동속도 감소시 최소 이동속도",

            StatId.Weapon_Attack                    => "공격력",
            StatId.Weapon_AttackSpeed               => "공격 속도",
            StatId.Weapon_ProjectileCount           => "발사 개수",
            StatId.Weapon_ProjectileRange           => "사거리",
            StatId.Weapon_ProjectileSpeed           => "탄속",
            StatId.Weapon_ProjectileAngle           => "탄퍼짐",

            StatId.Weapon_SpecialAttack             => "특수 공격력",
            StatId.Weapon_SpecialBeforeDelay        => "특수 선딜",
            StatId.Weapon_SpecialAfterDelay         => "특수 후딜",
            StatId.Weapon_SpecialProjectileCount    => "특수 발사 개수",
            StatId.Weapon_SpecialProjectileRange    => "특수 사거리",
            StatId.Weapon_SpecialProjectileSpeed    => "특수 탄속",

            StatId.Weapon_RifleMode => "라이플 변형 : ",
            StatId.Weapon_ShotgunMode => "샷건 변형 : ",
            StatId.Weapon_SniperMode => "저격총 변형 : ",

            _ => id.ToString()
        };
    }

    private static string FormatOp(StatId stat, ModOp op, float value)
    {
        if (stat == StatId.Weapon_RifleMode && op == ModOp.Override)
        {
            int v = Mathf.RoundToInt(value);
            string mode = v switch
            {
                1 => "무지성 난사",
                2 => "미니건",
                _ => "없음"
            };
            return $"{mode}";
        }

        if (stat == StatId.Weapon_ShotgunMode && op == ModOp.Override)
        {
            int v = Mathf.RoundToInt(value);
            string mode = v switch
            {
                1 => "슬러그탄",
                2 => "트리플샷",
                _ => "없음"
            };
            return $"{mode}";
        }

        if (stat == StatId.Weapon_SniperMode && op == ModOp.Override)
        {
            int v = Mathf.RoundToInt(value);
            string mode = v switch
            {
                1 => "도탄",
                2 => "땅땅땅빵",
                _ => "없음"
            };
            return $"{mode}";
        }

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
