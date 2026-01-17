using System.Collections.Generic;
using UnityEngine;

public class BossCannon : BulletBase
{
    [Header("VFX Prefabs")]
    [SerializeField, Tooltip("총구 화염 효과")] protected ParticleSystem _MuzzlePrefab;
    [SerializeField, Tooltip("폭발 파티클 프리팹")] protected ParticleSystem _ExplosionParticle;
    [SerializeField, Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] protected List<GameObject> _Trails;

    public override void OnSpawned()
    {
        base.OnSpawned();

        if (_MuzzlePrefab != null)
        {
            ParticleSystem muzzleVFX = PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);
            if (muzzleVFX != null)
            {
                muzzleVFX.Play();
            }
        }

        if (_Trails != null)
        {
            for (int i = 0; i < _Trails.Count; i++)
            {
                if (_Trails[i] == null) continue;

                _Trails[i].transform.parent = transform;
                _Trails[i].transform.localPosition = Vector3.zero;

                if (_Trails[i].TryGetComponent<ParticleSystem>(out var ps))
                    ps.Play();
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
            }

            HandleTrails();
            SpawnExplosion();
            Despawn();
        }
    }

    protected virtual void HandleTrails()
    {
        if (_Trails == null || _Trails.Count == 0)
            return;

        for (int i = 0; i < _Trails.Count; i++)
        {
            if (_Trails[i] == null)
                continue;

            _Trails[i].transform.parent = null;

            if (_Trails[i].TryGetComponent<ParticleSystem>(out var ps))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    protected virtual void SpawnExplosion()
    {
        if (_ExplosionParticle == null)
            return;

        ParticleSystem instance = PoolManager.Instance.Spawn(_ExplosionParticle, transform.position, transform.rotation);
        if (instance != null)
        {
            instance.Play();
        }
    }
}