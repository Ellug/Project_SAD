using System.Text;
using TMPro;
using UnityEngine;

public class StatsDebugCanvas : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private PlayerModel _player;
    [SerializeField] private TMP_Text _text;

    [Header("Options")]
    [SerializeField] private bool _showWeapon = true;
    [SerializeField] private bool _showInternal = true;
    [SerializeField] private float _refreshInterval = 0.1f;

    private PlayerStatsContext _ctx;
    private float _acc;

    private void Awake()
    {
        if (_player == null)
            _player = FindFirstObjectByType<PlayerModel>();

        if (_player != null)
            _ctx = _player.GetComponent<PlayerStatsContext>();

        if (_ctx != null)
            _ctx.OnChanged += RefreshNow;
    }

    private void OnDestroy()
    {
        if (_ctx != null)
            _ctx.OnChanged -= RefreshNow;
    }

    private void Update()
    {
        if (_text == null) return;

        if (_refreshInterval <= 0f)
        {
            RefreshNow();
            return;
        }

        _acc += Time.unscaledDeltaTime;
        if (_acc >= _refreshInterval)
        {
            _acc = 0f;
            RefreshNow();
        }
    }

    private void RefreshNow()
    {
        if (_text == null) return;

        if (_player == null)
        {
            _text.text = "[StatsDebugCanvas]\nPlayerModel이 없습니다.";
            return;
        }

        PlayerRuntimeStats pBase = _player.CaptureBaseStatsSnapshot();
        PlayerFinalStats final = _player.FinalStats;
        PlayerRuntimeStats pFinal = final.Player;

        WeaponRuntimeStats wBase = default;
        WeaponRuntimeStats wFinal = final.Weapon;

        WeaponBase weapon = _player.CurrentWeapon;
        if (weapon != null && weapon.WeaponData != null)
            wBase = WeaponRuntimeStats.FromData(weapon.WeaponData);

        var sb = new StringBuilder(2048);

        // Header
        sb.AppendLine("=== PLAYER STATS ===");

        if (_showInternal)
        {
            sb.AppendLine($"CurHp: {Fmt(_player.CurHp)} / {Fmt(_player.MaxHp)}");
            sb.AppendLine($"DodgeCD: {Fmt(_player.DodgeCooldownCur)} (ratio {Fmt(_player.DodgeCooldownRatio)})");
            sb.AppendLine($"SpecialCD: {Fmt(_player.SpecialCooldownCur)} (ratio {Fmt(_player.SpecialCooldownRatio)})");
            sb.AppendLine();
        }

        // Player Base / Final / Delta
        sb.AppendLine("[Player Base / Final / Δ(Final-Base)]");

        AppendFloatLine(sb, "MaxHp", pBase.MaxHp, pFinal.MaxHp);
        AppendFloatLine(sb, "MaxSpeed", pBase.MaxSpeed, pFinal.MaxSpeed);
        AppendFloatLine(sb, "AccelForce", pBase.AccelForce, pFinal.AccelForce);
        AppendFloatLine(sb, "RotSpeed", pBase.RotSpeed, pFinal.RotSpeed);

        AppendFloatLine(sb, "DodgeDuration", pBase.DodgeDuration, pFinal.DodgeDuration);
        AppendFloatLine(sb, "DodgeSpeed", pBase.DodgeSpeed, pFinal.DodgeSpeed);
        AppendFloatLine(sb, "DodgeCoolTime", pBase.DodgeCoolTime, pFinal.DodgeCoolTime);

        AppendFloatLine(sb, "SpecialCoolTime", pBase.SpecialCoolTime, pFinal.SpecialCoolTime);

        AppendFloatLine(sb, "AttackSlowRate", pBase.AttackSlowRate, pFinal.AttackSlowRate);

        sb.AppendLine();

        // Weapon
        if (_showWeapon)
        {
            sb.AppendLine("=== WEAPON STATS ===");

            if (weapon == null)
            {
                sb.AppendLine("CurrentWeapon: (null)");
            }
            else
            {
                sb.AppendLine($"CurrentWeapon: {weapon.name} (Id {weapon.GetWeaponId()})");
                sb.AppendLine();

                // Normal
                sb.AppendLine(" - Normal");
                AppendFloatLine(sb, "Attack", wBase.Attack, wFinal.Attack);
                AppendFloatLine(sb, "AttackSpeed", wBase.AttackSpeed, wFinal.AttackSpeed);
                AppendIntLine(sb, "ProjectileCount", wBase.ProjectileCount, wFinal.ProjectileCount);
                AppendFloatLine(sb, "ProjectileAngle", wBase.ProjectileAngle, wFinal.ProjectileAngle);
                AppendFloatLine(sb, "ProjectileRange", wBase.ProjectileRange, wFinal.ProjectileRange);
                AppendFloatLine(sb, "ProjectileSpeed", wBase.ProjectileSpeed, wFinal.ProjectileSpeed);
                AppendPrefabLine(sb, "ProjectilePrefab", wBase.ProjectilePrefab, wFinal.ProjectilePrefab);

                sb.AppendLine();

                // Special
                sb.AppendLine(" - Special");
                AppendFloatLine(sb, "SpecialAttack", wBase.SpecialAttack, wFinal.SpecialAttack);
                AppendFloatLine(sb, "SpecialBeforeDelay", wBase.SpecialAttackBeforeDelay, wFinal.SpecialAttackBeforeDelay);
                AppendFloatLine(sb, "SpecialAfterDelay", wBase.SpecialAttackAfterDelay, wFinal.SpecialAttackAfterDelay);
                AppendIntLine(sb, "SpecialProjectileCount", wBase.SpecialProjectileCount, wFinal.SpecialProjectileCount);
                AppendFloatLine(sb, "SpecialProjectileAngle", wBase.SpecialProjectileAngle, wFinal.SpecialProjectileAngle);
                AppendFloatLine(sb, "SpecialProjectileRange", wBase.SpecialProjectileRange, wFinal.SpecialProjectileRange);
                AppendFloatLine(sb, "SpecialProjectileSpeed", wBase.SpecialProjectileSpeed, wFinal.SpecialProjectileSpeed);
                AppendPrefabLine(sb, "SpecialProjectilePrefab", wBase.SpecialProjectilePrefab, wFinal.SpecialProjectilePrefab);
            }
        }

        _text.text = sb.ToString();
    }

    // Helpers
    private static void AppendFloatLine(StringBuilder sb, string name, float b, float f)
    {
        sb.Append(name).Append(": ");
        sb.Append(Fmt(b)).Append(" -> ").Append(Fmt(f));
        sb.Append(" ( ").Append(FmtSigned(f - b)).AppendLine(" ) ");
    }

    private static void AppendIntLine(StringBuilder sb, string name, int b, int f)
    {
        sb.Append(name).Append(": ");
        sb.Append(b).Append(" -> ").Append(f);
        sb.Append(" ( ").Append((f - b) >= 0 ? "+" : "").Append(f - b).AppendLine(" ) ");
    }

    private static void AppendPrefabLine<T>(StringBuilder sb, string name, T b, T f) where T : Object
    {
        string bn = (b == null) ? "null" : b.name;
        string fn = (f == null) ? "null" : f.name;

        sb.Append(name).Append(": ");
        sb.Append(bn).Append(" -> ").Append(fn);

        if (bn == fn) sb.AppendLine(" ( -)");
        else sb.AppendLine(" (changed)");
    }

    private static string Fmt(float v) => v.ToString("0.##");
    private static string FmtSigned(float v) => (v >= 0f ? "+" : "") + v.ToString("0.##");
}
