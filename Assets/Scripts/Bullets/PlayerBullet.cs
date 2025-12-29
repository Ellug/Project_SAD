using UnityEngine;

public enum BulletEffect
{
    None,
    Burn
}

public struct BulletEffectPayload
{
    // 화상 옵션
    public BulletEffect effect;
    public float burnDps;
    public float burnDuration;

    // 도탄 옵션
    public bool enableBounce;
    public int maxBounces;  // 튕김 횟수
    public float bouncedDamageMul; // 튕긴 뒤 적중시 대미지 배율

    // 단순 배율
    public float damageMul; // 기본1, for 땅땅땅빵 경우 4배 적용

    // 최대 체력 비례 추가 피해 (0.01 = 1%)
    public float dmgPerMaxHp;
}

public class PlayerBullet : BulletBase
{
    private bool _counterAttack;
    private BulletEffectPayload _payload;

    // Bounce State
    private int _bounceRemain;
    private bool _hasBounced;
    private Vector3 _prevPos;
    private float _bounceSkin = 0.02f;
    private Collider _selfCol;

    [SerializeField] protected ParticleSystem flash;
    [SerializeField] protected ParticleSystem hit;
    [SerializeField] protected Light lightSourse;
    [SerializeField] protected GameObject[] Detached;
    [SerializeField] protected ParticleSystem projectilePS;
    [SerializeField] protected float hitOffset = 0f;

    protected override void Awake()
    {
        base.Awake();
        _selfCol = GetComponent<Collider>();
    }

    public void Init(WeaponRuntimeStats stats, bool isSpecial = false, BulletEffectPayload payload = default)
    {
        _counterAttack = isSpecial;
        _payload = payload;

        // payload 기본값 보정
        if (_payload.damageMul <= 0f)
            _payload.damageMul = 1f;

        if (_payload.bouncedDamageMul <= 0f)
            _payload.bouncedDamageMul = 1f;

        _bounceRemain = _payload.enableBounce ? Mathf.Max(0, _payload.maxBounces) : 0;
        _hasBounced = false;

        // 특수탄이면 Special 스탯 사용
        float dmg = isSpecial ? 0f : stats.Attack;
        float speed = isSpecial ? stats.SpecialProjectileSpeed : stats.ProjectileSpeed;
        float range = isSpecial ? stats.SpecialProjectileRange : stats.ProjectileRange;

        base.Init(dmg: dmg, speed: speed, maxDistance: range);

        _prevPos = transform.position;
    }

    public override void OnSpawned()
    {
        base.OnSpawned();

        if (flash != null)
        {
            ParticleSystem muzzleVFX =
                PoolManager.Instance.Spawn(flash, transform.position, transform.rotation);

            if (muzzleVFX != null)
                muzzleVFX.Play();
        }

        if (lightSourse != null)
            lightSourse.enabled = true;

        if (projectilePS != null)
            projectilePS.Play();
    }

    public override void OnDespawned()
    {
        base.OnDespawned();

        _counterAttack = false;
        _payload = default;
        _bounceRemain = 0;
        _hasBounced = false;
        _prevPos = Vector3.zero;

        if (lightSourse != null)
            lightSourse.enabled = false;
    }

    protected override void MoveForward()
    {
        _prevPos = transform.position;
        base.MoveForward();
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 normal = transform.position - hitPoint;
        normal.y = 0f;
        if (normal.sqrMagnitude < 0.0001f) normal = -transform.forward;

        // Obstacle -> Bounce or Despawn
        if (other.CompareTag("Obstacle"))
        {
            if (_payload.enableBounce && _bounceRemain > 0)
            {
                Bounce(other);
                return;
            }

            PlayHitFX(hitPoint, normal);
            Despawn();
            return;
        }

        // Boss -> Dmg + OnHit Effect
        if (other.transform.CompareTag("Boss"))
        {
            if (other.TryGetComponent<BossController>(out var boss))
            {
                float dmg = _dmg * _payload.damageMul;

                if (_hasBounced)
                    dmg *= _payload.bouncedDamageMul;

                // 최대체력 비례 추가 피해 (payload로 전달된 pct 사용)
                if (_payload.dmgPerMaxHp > 0f)
                {
                    float pct = Mathf.Clamp01(_payload.dmgPerMaxHp);
                    dmg += boss.BossMaxHp * pct;
                }

                boss.TakeDamage(dmg, _counterAttack);

                if (_payload.effect == BulletEffect.Burn)
                    boss.ApplyBurn(_payload.burnDps, _payload.burnDuration);
            }

            PlayHitFX(hitPoint, normal);
            Despawn();
            return;
        }
    }

    private void PlayHitFX(Vector3 point, Vector3 normal)
    {
        if (lightSourse != null)
            lightSourse.enabled = false;

        if (projectilePS != null)
            projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        foreach (var d in Detached)
        {
            if (d != null && d.TryGetComponent<ParticleSystem>(out var ps))
                ps.Stop();
        }

        if (hit != null)
        {
            ParticleSystem hitVFX = PoolManager.Instance.Spawn(
                hit,
                point + normal * hitOffset,
                Quaternion.LookRotation(normal)
            );

            if (hitVFX != null)
                hitVFX.Play();
        }
    }

    // 도탄
    private void Bounce(Collider obstacle)
    {
        _bounceRemain--;
        _hasBounced = true;

        Vector3 travel = transform.position - _prevPos;
        if (travel.sqrMagnitude < 0.000001f)
            travel = transform.forward * 0.01f;

        // 입사각 벡터
        Vector3 incident = travel.normalized;

        // Ray가 collider 내부에서 시작하면 Collider.Raycast가 실패하기 쉬움
        float back = 0.05f;
        Vector3 rayOrigin = _prevPos - incident * back;
        float rayDist = travel.magnitude + back + 0.1f;

        Vector3 normal;
        Vector3 hitPoint;

        Ray ray = new(rayOrigin, incident);

        // 1) 표면 법선 우선 확보
        if (obstacle.Raycast(ray, out RaycastHit hit, rayDist))
        {
            normal = hit.normal;
            hitPoint = hit.point;
        }
        else
        {
            // 2) Raycast 실패 시: penetration으로 분리 방향을 법선처럼 사용 => 겹침 상태 처리
            if (_selfCol != null &&
                Physics.ComputePenetration(
                    _selfCol, transform.position, transform.rotation,
                    obstacle, obstacle.transform.position, obstacle.transform.rotation,
                    out Vector3 sepDir, out float sepDist))
            {
                normal = sepDir;
                hitPoint = transform.position; // 임시
                // 분리 방향으로 충분히 밖으로 빼줌
                transform.position += sepDir * (sepDist + _bounceSkin);
            }
            else
            {
                // 3) 최후의 fallback: 진행방향 그냥 반대로
                normal = -incident;
                hitPoint = transform.position;
            }
        }

        // 안전 처리
        normal.y = 0f;
        if (normal.sqrMagnitude < 0.000001f)
            normal = Vector3.up;

        normal.Normalize();

        Vector3 reflected = Vector3.Reflect(incident, normal);
        reflected.y = 0f;

        if (reflected.sqrMagnitude < 0.000001f)
            reflected = incident;

        reflected.Normalize();

        // 즉시 재충돌 방지용으로 살짝 밀어냄
        transform.position = hitPoint + normal * _bounceSkin;
        transform.rotation = Quaternion.LookRotation(reflected, Vector3.up);
    }
}
