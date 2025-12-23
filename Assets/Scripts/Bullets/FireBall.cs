using UnityEngine;

public class FireBall : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;
    [Tooltip("폭발 파티클")][SerializeField] ParticleSystem _ExplosionParticle;
    [Tooltip("그을음  프리팹")] public BurnDecal BurnDecalPrefab;
    [Tooltip("파이어볼 판정 범위")][SerializeField] float _FireBallRange;
    [Tooltip("파이어볼 지속 시간")][SerializeField] float _FireBallLifeTime;
    [Tooltip("파이어볼 이동 속도")][SerializeField] float _FireBallSpeed = 15f;
    [Tooltip("데미지")][SerializeField] float _Dmg;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("충돌 레이어")][SerializeField] private LayerMask Layer;
    private ParticleSystem _Explosion;

    void Awake()
    {
        _poolMember = GetComponent<PoolMember>();
        if (_FireBallLifeTime != 0)
        {
            Invoke("DespawnFireBall", _FireBallLifeTime);
        }
    }

    void Update()
    {
        if (Player == null) return;

        transform.Translate(Vector3.forward * (_FireBallSpeed * Time.deltaTime), Space.Self);

        bool isHit = Physics.CheckSphere(transform.position, _FireBallRange, Player.layer);
        if (isHit)
        {
            Player.TryGetComponent<PlayerModel>(out var player);
            player.TakeDamage(_Dmg);
            DespawnFireBall();
        }

        bool objectHit = Physics.CheckSphere(transform.position, _FireBallRange, Layer);
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
    public void OnSpawned()
    {
    }

    public void OnDespawned()
    {
    }
}
