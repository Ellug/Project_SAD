using UnityEngine;

public enum SniperPerkMode
{
    None,
    Bouncing,
    CurtainCall
}

public class Sniper : WeaponBase
{
    [Header("Bouncing")]
    [SerializeField] private int _maxBounces = 3;
    [SerializeField] private float _bouncedDamageMul = 3f;

    [Header("CurtainCall")]
    [SerializeField] private int _shotsPerCycle = 4;
    [SerializeField] private float _fourthShotMul = 2f;

    private int _cycleShotCount;
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
        FireSound(_statsContext.Current.Weapon, false);

        _statsContext?.NotifyAttackSlow();
        return true;
    }

    private void FireDefault()
    {
        // 기본 저격탄 발사
        FireSniperBullet(payload: default);
    }

    private void FireBouncing()
    {
        BulletEffectPayload payload = new()
        {
            enableBounce = true,
            maxBounces = _maxBounces,
            bouncedDamageMul = _bouncedDamageMul,
            damageMul = 1f
        };

        // 도탄 기능 총알 발사
        FireSniperBullet(payload);
    }

    private void FireCurtainCall()
    {
        _cycleShotCount++;

        bool isBuffShot = _cycleShotCount >= _shotsPerCycle;

        BulletEffectPayload payload = new() { damageMul = isBuffShot ? _fourthShotMul : 1f };
        
        FireSniperBullet(payload);

        if (isBuffShot)
            _cycleShotCount = 0;
    }

    private void FireSniperBullet(BulletEffectPayload payload)
    {
        if (_statsContext == null) return;

        WeaponRuntimeStats stats = _statsContext.Current.Weapon;

        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        Vector3 spawnPos = transform.position + dir * 0.5f;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        PlayerBullet bullet = PoolManager.Instance.Spawn(stats.ProjectilePrefab, spawnPos, rot);
        bullet.Init(stats, false, payload);
    }

    private void Clear()
    {
        if (_statsContext != null)
            _statsContext.SetDynamicMods(_dynOwnerId, null);
    }
}
