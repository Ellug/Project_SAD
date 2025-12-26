using UnityEngine;

public enum ShotgunPerkMode
{
    None,
    Slug,
    Tripple
}

public class Shotgun : WeaponBase
{
    [Header("Tripple Shot Burn Op")]
    [SerializeField] private float _burnDps = 5f;
    [SerializeField] private float _burnDuration = 5f;

    private int _dynOwnerId;

    private ShotgunPerkMode CurrentMode
    {
        get
        {
            if (_statsContext == null) return ShotgunPerkMode.None;
            return (ShotgunPerkMode)_statsContext.Current.Weapon.ShotgunMode; // 0 1 2
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
            case ShotgunPerkMode.Slug: // 탄퍼짐은 퍽즈 노드 옵션에서 적용
                FireProjectile(false);
                break;

            case ShotgunPerkMode.Tripple:
                FireTriple();
                break;

            default:
                FireProjectile(false);
                break;
        }
        FireSound(_statsContext.Current.Weapon, false);

        _statsContext?.NotifyAttackSlow();
        return true;
    }

    private void FireTriple()
    {
        FireProjectile(false);

        if (_statsContext == null) return;

        WeaponRuntimeStats stats = _statsContext.Current.Weapon;

        // 탄환에 화상 payload 추가
        BulletEffectPayload burn = new()
        {
            effect = BulletEffect.Burn,
            burnDps = _burnDps,
            burnDuration = _burnDuration
        };

        int count = Mathf.Max(1, stats.ProjectileCount);
        float totalAngle = stats.ProjectileAngle;

        Vector3 baseDir = transform.forward;
        baseDir.y = 0f;
        baseDir.Normalize();

        Vector3 spawnPos = transform.position + baseDir * 0.5f;

        float halfAngle = totalAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;
            SpawnTripleBurnBullet(spawnPos, dir, stats, burn);
        }
    }

    private void SpawnTripleBurnBullet(Vector3 pos, Vector3 dir, WeaponRuntimeStats stats, BulletEffectPayload burn)
    {
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        PlayerBullet bullet = PoolManager.Instance.Spawn(stats.ProjectilePrefab, pos, rot);
        bullet.Init(stats, counterAttack: false, payload: burn);
    }

    private void Clear()
    {
        if (_statsContext != null)
            _statsContext.SetDynamicMods(_dynOwnerId, null);
    }
}
