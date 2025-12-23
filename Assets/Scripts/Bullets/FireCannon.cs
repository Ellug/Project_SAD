using UnityEngine;

public class FireCannon : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;
    [Tooltip("폭발 파티클")][SerializeField] ParticleSystem _ExplosionParticle;
    [Tooltip("그을음  프리팹")] public BurnDecal BurnDecalPrefab;
    [Tooltip("화염포 판정 범위")][SerializeField] float _FireCannonRange;
    [Tooltip("화염포 지속 시간")][SerializeField] float _FireCannonLifeTime;
    [Tooltip("화염포 이동 속도")][SerializeField] float _FireCannonSpeed = 15f;
    [Tooltip("데미지")][SerializeField] float _Dmg;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("충돌 레이어")][SerializeField] private LayerMask Layer;
    private ParticleSystem _Explosion;

    void Awake()
    {
        _poolMember = GetComponent<PoolMember>();
        if (_FireCannonLifeTime != 0)
        {
            Invoke("DespawnFireBall", _FireCannonLifeTime);
        }
    }

    void Update()
    {
        if (Player == null) return;

        transform.Translate(Vector3.forward * (_FireCannonSpeed * Time.deltaTime), Space.Self);

        bool isHit = Physics.CheckSphere(transform.position, _FireCannonRange, Player.layer);
        if (isHit)
        {
            Player.TryGetComponent<PlayerModel>(out var player);
            player.TakeDamage(_Dmg);
            DespawnFireBall();
        }

        bool objectHit = Physics.CheckSphere(transform.position, _FireCannonRange, Layer);
        if (objectHit)
        {
            BurnDecal dec = PoolManager.Instance.Spawn(BurnDecalPrefab, transform.position, transform.rotation);
            DespawnFireBall();
        }
    }

    private void DespawnFireBall()
    {
        _Explosion = Instantiate(_ExplosionParticle, transform.position, transform.rotation);
        var main = _Explosion.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        _Explosion.Play();
        PoolManager.Instance.Despawn(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (transform.position == null) return;

        // 실제 로직과 동일한 체크 수행 (디버그용)
        bool isHit = Physics.CheckSphere(transform.position, _FireCannonRange, Layer);

        // 감지되면 초록색, 아니면 빨간색
        Gizmos.color = isHit ? Color.green : Color.red;

        // 원 그리기
        Gizmos.DrawWireSphere(transform.position, _FireCannonRange);
    }
    public void OnSpawned()
    {
    }

    public void OnDespawned()
    {
    }
}
