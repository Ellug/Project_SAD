using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponData _weaponData;

    [Header("Perks")]
    [SerializeField] private PerksTree _perksTree;

    protected PlayerStatsContext _statsContext;
    private Coroutine _specialAttackRoutine;

    public WeaponData WeaponData => _weaponData;
    public PerksTree PerksTree => _perksTree;

    protected virtual void Awake()
    {
        if (_statsContext == null)
            _statsContext = GetComponentInParent<PlayerStatsContext>();
    }

    public int GetWeaponId()
    {
        return _weaponData.WeaponId;
    }

    public virtual bool TryAttack()
    {
        FireProjectile(false);

        // 공격 감속 트리거는 Context를 통해 PlayerModel로 전달
        if (_statsContext != null)
            _statsContext.NotifyAttackSlow();

        return true;
    }

    public void SpecialAttack()
    {
        if (_specialAttackRoutine != null)
            StopCoroutine(_specialAttackRoutine);

        _specialAttackRoutine = StartCoroutine(CoSpecialAttack());
    }

    public void CancelSpecialAttack()
    {
        if (_specialAttackRoutine != null)
        {
            StopCoroutine(_specialAttackRoutine);
            _specialAttackRoutine = null;
        }

        if (_statsContext != null)
            _statsContext.NotifySpecialAttackState(false);
    }

    private IEnumerator CoSpecialAttack()
    {
        if (_statsContext == null)
            yield break;

        WeaponRuntimeStats stats = _statsContext.Current.Weapon;

        yield return StartCoroutine(CoBeforeSpecialAttack(stats));
        FireProjectile(true);
        yield return StartCoroutine(CoAfterSpecialAttack(stats));

        _specialAttackRoutine = null;
    }

    private IEnumerator CoBeforeSpecialAttack(WeaponRuntimeStats stats)
    {
        _statsContext.NotifySpecialAttackState(true);
        yield return new WaitForSeconds(stats.SpecialAttackBeforeDelay);
    }

    private IEnumerator CoAfterSpecialAttack(WeaponRuntimeStats stats)
    {
        yield return new WaitForSeconds(stats.SpecialAttackAfterDelay);
        _statsContext.NotifySpecialAttackState(false);
    }

    //불릿 정보 줄거
    protected void FireProjectile(bool isSpecial)
    {
        if (_statsContext == null) return;

        WeaponRuntimeStats stats = _statsContext.Current.Weapon;

        int count = Mathf.Max(1, isSpecial ? stats.SpecialProjectileCount : stats.ProjectileCount);
        float totalAngle = isSpecial ? stats.SpecialProjectileAngle : stats.ProjectileAngle;

        Vector3 baseDir = transform.forward;
        baseDir.y = 0f;
        baseDir.Normalize();

        Vector3 spawnPos = transform.position + baseDir * 0.5f;

        // 각도, 숫자로 산탄 계산
        if (count == 1 || totalAngle == 0f)
        {
            SpawnBullet(spawnPos, baseDir, stats, isSpecial);
            return;
        }

        float halfAngle = totalAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;
            SpawnBullet(spawnPos, dir, stats, isSpecial);
        }
    }

    protected void SpawnBullet(Vector3 pos, Vector3 dir, WeaponRuntimeStats stats, bool isSpecial)
    {
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        
        PlayerBullet prefab = isSpecial ? stats.SpecialProjectilePrefab : stats.ProjectilePrefab;
        PlayerBullet bullet = PoolManager.Instance.Spawn(prefab, pos, rot);

        BulletEffectPayload payload = default;

        if (isSpecial)
            payload.dmgPerMaxHp = stats.SpecialAttack;

        bullet.Init(stats, isSpecial, payload);
    }
}
