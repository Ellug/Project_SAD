using System.Collections.Generic;
using UnityEngine;

public class FireCannon : BulletBase
{
    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public GameObject _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public GameObject _ExplosionParticle;
    [Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] public List<GameObject> _Trails;

    [Header("데칼 설정")]
    [Tooltip("그을음 데칼 프리팹")] public BurnDecal _BurnDecalPrefab;

    [Header("화상 설정")]
    [Tooltip("화상 데미지")] public float BurnDebuffDmg = 5f;
    [Tooltip("화상 지속 시간")] public float BurnDebuffTime = 2f;
    [Tooltip("화상 틱 인터벌")] public float BurnDebuffInterval = 0.1f;


    public override void OnSpawned()
    {
        base.OnSpawned();

        if (_MuzzlePrefab != null)
        {
            GameObject muzzleVFX = Instantiate(_MuzzlePrefab, transform.position, transform.rotation);
            var ps = muzzleVFX.GetComponent<ParticleSystem>() ?? muzzleVFX.GetComponentInChildren<ParticleSystem>();
            if (ps != null) Destroy(muzzleVFX, ps.main.duration);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerModel>(out var player))
            {
                player.TakeDamage(_dmg);
                player.BurnDebuff(BurnDebuffDmg, BurnDebuffTime, BurnDebuffInterval);
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

    private void HandleTrails()
    {
        if (_Trails != null && _Trails.Count > 0)
        {
            for (int i = 0; i < _Trails.Count; i++)
            {
                if (_Trails[i] == null) continue;

                _Trails[i].transform.parent = null;
                var ps = _Trails[i].GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(_Trails[i], ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }
    }

    private void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;

        GameObject instance = Instantiate(_ExplosionParticle, transform.position, transform.rotation);
        var ps = instance.GetComponent<ParticleSystem>() ?? instance.GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            ps.Play();
        }
    }
}