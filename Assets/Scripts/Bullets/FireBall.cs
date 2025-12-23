using TMPro;
using UnityEngine;

public class FireBall : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;
    [Tooltip("폭발 파티클")][SerializeField] ParticleSystem _ExplosionParticle;
    [Tooltip("화염장판 오브젝트")][SerializeField] FireArea _FireAreaPrefab;
    [Tooltip("그을음  프리팹")] public BurnDecal BurnDecalPrefab;
    [Tooltip("화염구 판정 범위")][SerializeField] float _FireCannonRange;
    [Tooltip("화염구 이동 속도")][SerializeField] float _FireCannonSpeed = 15f;
    [Tooltip("충돌 레이어")][SerializeField] private LayerMask Layer;
    private ParticleSystem _Explosion;
    private Transform FireTargetPoint;

    private void Update()
    {
        if (FireTargetPoint == null) return;

        transform.position = Vector3.MoveTowards(transform.position, FireTargetPoint.position, _FireCannonSpeed * Time.deltaTime);

        bool TargetHit = Physics.CheckSphere(transform.position, _FireCannonRange, Layer);
        if (TargetHit)
        {
            FireArea fireArea = PoolManager.Instance.Spawn(_FireAreaPrefab, FireTargetPoint.position, FireTargetPoint.rotation);
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

    public void setTarget(Transform transform) 
    {
        FireTargetPoint = transform;
    }
    public void OnSpawned()
    {
    }

    public void OnDespawned()
    {
    }
}
