using UnityEngine;
using System.Collections.Generic;

public class GuidedMissile : MonoBehaviour, IPoolable
{
    [Header("VFX 프리팹")]
    [SerializeField, Tooltip("총구 화염 효과")] protected ParticleSystem _MuzzlePrefab;
    [SerializeField, Tooltip("폭발 파티클")] protected ParticleSystem _ExplosionPrefab;
    [SerializeField, Tooltip("잔상(Trail) 리스트")] protected List<ParticleSystem> _Trails;

    protected float _MissileSpeed;
    protected float _MissileRotationSpeed;
    protected float _MissileLifeTime;
    protected float _Dmg;

    protected GameObject _target;
    protected bool _isDespawning;

    public void Init(GameObject target, float speed, float rotSpeed, float lifeTime, float dmg)
    {
        _target = target;
        _MissileSpeed = speed;
        _MissileRotationSpeed = rotSpeed;
        _MissileLifeTime = lifeTime;
        _Dmg = dmg;
        _isDespawning = false;

        SpawnMuzzleFlash();

        if (_MissileLifeTime > 0)
            Invoke(nameof(DespawnMissile), _MissileLifeTime);
    }

    protected virtual void Update()
    {
        if (_target == null || _isDespawning) return;

        Vector3 dir = (_target.transform.position - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * _MissileRotationSpeed);
        }

        transform.Translate(Vector3.forward * _MissileSpeed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (_isDespawning) return;

        if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerModel>(out var player))
            {
                player.TakeDamage(_Dmg);
            }

            DespawnMissile();
        }
    }

    protected virtual void DespawnMissile()
    {
        if (_isDespawning) return;
        _isDespawning = true;

        CancelInvoke(nameof(DespawnMissile));
        HandleTrails();
        SpawnExplosion();

        PoolManager.Instance.Despawn(gameObject);
    }

    protected virtual void SpawnMuzzleFlash()
    {
        if (_MuzzlePrefab == null) return;
        var ps = PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);
        if (ps != null) ps.Play();
    }

    protected virtual void SpawnExplosion()
    {
        if (_ExplosionPrefab == null) return;
        var ps = PoolManager.Instance.Spawn(_ExplosionPrefab, transform.position, transform.rotation);
        if (ps != null) ps.Play();
    }

    protected virtual void HandleTrails()
    {
        if (_Trails == null) return;
        foreach (var ps in _Trails)
        {
            if (ps == null) continue;
            ps.transform.parent = null;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public virtual void OnSpawned() { }

    public virtual void OnDespawned()
    {
        _isDespawning = false;
        if (_Trails == null) return;
        foreach (var ps in _Trails)
        {
            if (ps != null) ps.transform.SetParent(transform, false);
        }
    }
}