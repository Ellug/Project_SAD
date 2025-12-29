using System.Collections.Generic;
using UnityEngine;

public class FireCannon : BulletBase
{
    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public ParticleSystem _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public ParticleSystem _ExplosionParticle;
    [Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] public List<ParticleSystem> _Trails;

    [Header("데칼 설정")]
    [Tooltip("그을음 데칼 프리팹")] public BurnDecal _BurnDecalPrefab;

    [Header("화상 설정")]
    [Tooltip("화상 데미지")] public float BurnDebuffDmg = 5f;
    [Tooltip("화상 지속 시간")] public float BurnDebuffTime = 2f;
    [Tooltip("화상 틱 인터벌")] public float BurnDebuffInterval = 0.1f;

    void Start()
    {
        if (_MuzzlePrefab != null)
        {
            ParticleSystem muzzle =
                PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);

            if (muzzle != null)
            {
                muzzle.Play();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") &&
                other.TryGetComponent<PlayerModel>(out var player))
            {
                player.TakeDamage(_dmg);
                player.BurnDebuff(BurnDebuffDmg, BurnDebuffTime, BurnDebuffInterval);
            }

            if (other.CompareTag("Obstacle") && _BurnDecalPrefab != null)
            {
                PoolManager.Instance.Spawn(
                    _BurnDecalPrefab,
                    transform.position,
                    transform.rotation
                );
            }

            HandleTrails();
            SpawnExplosion();
            Despawn();
        }
    }

    private void HandleTrails()
    {
        if (_Trails == null || _Trails.Count == 0) return;

        for (int i = 0; i < _Trails.Count; i++)
        {
            if (_Trails[i] == null) continue;

            _Trails[i].transform.parent = null;
            _Trails[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;

        ParticleSystem explosion =
            PoolManager.Instance.Spawn(
                _ExplosionParticle,
                transform.position,
                transform.rotation
            );

        if (explosion != null)
        {
            explosion.Play();
        }
    }
}
