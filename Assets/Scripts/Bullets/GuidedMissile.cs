using UnityEngine;
using System.Collections.Generic;

public class GuidedMissile : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;

    [Header("VFX 프리팹")]
    [Tooltip("총구 화염 효과 프리팹")][SerializeField] private GameObject _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")][SerializeField] private GameObject _ExplosionPrefab;
    [Tooltip("미사일에 붙어있는 잔상(Trail) 리스트")] public List<GameObject> _Trails;

    [Header("미사일 설정")]
    [Tooltip("미사일 지속 시간")][SerializeField] private float _MissileLifeTime = 5f;
    [Tooltip("미사일 이동 속도")][SerializeField] private float _MissileSpeed = 15f;
    [Tooltip("미사일 회전 속도")][SerializeField] private float _MissileRotationSpeed = 3f;
    [Tooltip("데미지")][SerializeField] private float _Dmg;

    [Header("플레이어")]
    [Tooltip("플레이어")][SerializeField] private GameObject Player;

    private bool _isDespawning = false;

    private void Awake() 
    {
        Player = GameObject.FindWithTag("Player");
    }

    public void OnSpawned()
    {
        _isDespawning = false;

        SpawnMuzzleFlash();

        if (_MissileLifeTime > 0)
        {
            Invoke(nameof(DespawnMissile), _MissileLifeTime);
        }
    }

    void Update()
    {
        if (Player == null || _isDespawning) return;

        Vector3 direction = (Player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _MissileRotationSpeed);

        transform.Translate(Vector3.forward * _MissileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
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

    private void DespawnMissile()
    {
        if (_isDespawning) return;
        _isDespawning = true;

        CancelInvoke(nameof(DespawnMissile));

        HandleTrails();

        SpawnExplosion();

        PoolManager.Instance.Despawn(gameObject);
    }

    private void SpawnMuzzleFlash()
    {
        if (_MuzzlePrefab == null) return;

        GameObject muzzleVFX = Instantiate(_MuzzlePrefab, transform.position, transform.rotation);
        float duration = GetParticleDuration(muzzleVFX);
        Destroy(muzzleVFX, duration);
    }

    private void SpawnExplosion()
    {
        if (_ExplosionPrefab == null) return;

        GameObject explosionVFX = Instantiate(_ExplosionPrefab, transform.position, transform.rotation);

        var ps = explosionVFX.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            ps.Play();
        }
        else
        {
            Destroy(explosionVFX, GetParticleDuration(explosionVFX));
        }
    }

    private void HandleTrails()
    {
        foreach (var trail in _Trails)
        {
            if (trail == null) continue;

            trail.transform.parent = null;
            var ps = trail.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                Destroy(trail, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    private float GetParticleDuration(GameObject vfx)
    {
        var ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null) return ps.main.duration;

        if (vfx.transform.childCount > 0)
        {
            var childPS = vfx.transform.GetChild(0).GetComponent<ParticleSystem>();
            if (childPS != null) return childPS.main.duration;
        }
        return 2f;
    }

    public void OnDespawned() 
    { 
    }
}