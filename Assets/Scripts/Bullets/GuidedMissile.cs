using UnityEngine;
using UnityEngine.UIElements;

public class GuidedMissile : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;
    [Tooltip("폭발 파티클")][SerializeField] ParticleSystem _ExplosionParticle;
    [Tooltip("미사일 판정 범위")][SerializeField] float _MisssileRange;
    [Tooltip("미사일 지속 시간")][SerializeField] float _MisssileLifeTime;
    [Tooltip("미사일 이동 속도")][SerializeField] float _MisssileSpeed = 15f;
    [Tooltip("미사일 회전 속도")][SerializeField] float _MisssileRotationSpeed = 3f;
    [Tooltip("데미지")][SerializeField] float _Dmg;
    [Tooltip("플레이어")][SerializeField] private GameObject Player;
    [Tooltip("충돌 레이어")][SerializeField] private LayerMask Layer;
    private ParticleSystem _Explosion;

    void Awake() 
    {
        _poolMember = GetComponent<PoolMember>();
        if (_MisssileLifeTime != 0) 
        {
            Invoke("DespawnMissile", _MisssileLifeTime);
        }
    }

    void Update() 
    {
        if (Player == null) return;

        Vector3 direction = (Player.transform.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _MisssileRotationSpeed);

        transform.Translate(Vector3.forward * _MisssileSpeed * Time.deltaTime);

        bool isHit = Physics.CheckSphere(transform.position, _MisssileRange, Player.layer);
        if (isHit)
        {
            Player.TryGetComponent<PlayerModel>(out var player);
            player.TakeDamage(_Dmg);
            DespawnMissile();
        }

        bool objectHit = Physics.CheckSphere(transform.position, _MisssileRange, Layer);
        if (objectHit)
        {
            DespawnMissile();
        }
    }

    private void DespawnMissile() 
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
