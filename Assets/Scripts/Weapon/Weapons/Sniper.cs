using UnityEngine;

public enum SniperPerkMode
{
    None,
    Bouncing,
    CurtainCall
}

public class Sniper : WeaponBase
{
    private int _dynOwnerId;

    private SniperPerkMode CurrentMode
    {
        get
        {
            if (_statsContext == null) return SniperPerkMode.None;
            return (SniperPerkMode)_statsContext.Current.Weapon.SniperMode; // 0 1 2
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _dynOwnerId = GetInstanceID(); // int 키
    }

    private void OnDisable()
    {
        Clear();
    }

    // 공격 시도
    public override bool TryAttack()
    {
        var mode = CurrentMode;

        switch (mode)
        {
            case SniperPerkMode.Bouncing:
                FireBouncing();
                break;

            case SniperPerkMode.CurtainCall:
                FireCurtainCall();
                break;

            default:
                FireDefault();
                break;
        }

        _statsContext?.NotifyAttackSlow();
        return true;
    }

    private void FireDefault()
    {
        // 기본 저격탄 발사
        FireProjectile(false);
    }

    private void FireBouncing()
    {
        // 도탄 기능 총알 발사
        FireProjectile(false);
    }

    private void FireCurtainCall()
    {
        // 4번째 총알 강화 개념 도입
        FireProjectile(false);
    }

    private void Clear()
    {
        if (_statsContext != null)
            _statsContext.SetDynamicMods(_dynOwnerId, null);
    }
}
