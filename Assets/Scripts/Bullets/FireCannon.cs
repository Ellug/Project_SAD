using System.Collections.Generic;
using UnityEngine;

public class FireCannon : BulletBase
{
    [Header("VFX Prefabs")]
    [SerializeField, Tooltip("총구 화염 효과")] protected ParticleSystem _MuzzlePrefab;
    [SerializeField, Tooltip("폭발 파티클 프리팹")] protected ParticleSystem _ExplosionParticle;
    [SerializeField, Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] protected List<ParticleSystem> _Trails;

    [Header("데칼 설정")]
    [SerializeField, Tooltip("그을음 데칼 프리팹")] protected BurnDecal _BurnDecalPrefab;

    protected float _burnDmg;
    protected float _burnTime;
    protected float _burnInterval;

    public void SetBurnStats(float dmg, float time, float interval)
    {
        _burnDmg = dmg;
        _burnTime = time;
        _burnInterval = interval;
    }
    public override void OnSpawned()
    {
        base.OnSpawned();

        if (_MuzzlePrefab != null)
        {
            ParticleSystem muzzle = PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);
            if (muzzle != null) muzzle.Play();
        }

        if (_Trails != null)
        {
            foreach (var trail in _Trails)
            {
                if (trail == null) continue;
                trail.transform.parent = transform;
                trail.transform.localPosition = Vector3.zero;
                trail.Play();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerModel>(out var player))
            {
                player.TakeDamage(_dmg);
                player.BurnDebuff(_burnDmg, _burnTime, _burnInterval);
            }

            if (other.CompareTag("Obstacle") && _BurnDecalPrefab != null)
            {
                PoolManager.Instance.Spawn(_BurnDecalPrefab, transform.position, transform.rotation);
            }

            HandleTrails();
            SpawnExplosion();
            Despawn();
        }
    }

    protected virtual void HandleTrails()
    {
        if (_Trails == null) return;

        foreach (var trail in _Trails)
        {
            if (trail == null) continue;
            trail.transform.parent = null;
            trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    protected virtual void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;

        ParticleSystem explosion = PoolManager.Instance.Spawn(_ExplosionParticle, transform.position, transform.rotation);
        if (explosion != null) explosion.Play();
    }
}