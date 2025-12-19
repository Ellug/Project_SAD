public class PlayerFinalStats
{
    // 최종적으로 사용될 런타임 스탯 스냅샷
    // - Player / Weapon 런타임 스탯은 Final 안에서 함께 관리
    // - 실제 변경/계산 책임은 CombatStatsContext가 담당
    public PlayerRuntimeStats Player;
    public WeaponRuntimeStats Weapon;

    // Derived
    public float AttackCoolTime { get; private set; }

    public void UpdateDerived()
    {
        AttackCoolTime = (Weapon.AttackSpeed > 0f) ? (1f / Weapon.AttackSpeed) : 0f;
    }
}
