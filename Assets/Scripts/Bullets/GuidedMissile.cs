using UnityEngine;
using System.Collections.Generic;

public class GuidedMissile : MonoBehaviour, IPoolable
{
    private PoolMember _poolMember;

    [Header("VFX 프리팹")]
    [Tooltip("총구 화염 효과 프리팹")][SerializeField] private ParticleSystem _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")][SerializeField] private ParticleSystem _ExplosionPrefab;
    [Tooltip("미사일에 붙어있는 잔상(Trail) 리스트")] public List<ParticleSystem> _Trails;

    [Header("미사일 설정")]
    [Tooltip("미사일 지속 시간")][SerializeField] private float _MissileLifeTime = 5f;
    [Tooltip("미사일 이동 속도")][SerializeField] private float _MissileSpeed = 15f;
    [Tooltip("미사일 회전 속도")][SerializeField] private float _MissileRotationSpeed = 3f;
    [Tooltip("데미지")][SerializeField] private float _Dmg;

    [Header("플레이어")]
    [Tooltip("플레이어")][SerializeField] private GameObject Player;

    private bool _isDespawning;

    private void Awake()
    {
        Player = GameObject.FindWithTag("Player");
    }

    public void OnSpawned()
    {
        _isDespawning = false;

        SpawnMuzzleFlash();

        if (_MissileLifeTime > 0)
            Invoke(nameof(DespawnMissile), _MissileLifeTime);
    }

    void Update()
    {
        if (Player == null || _isDespawning) return;

        Vector3 dir = (Player.transform.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * _MissileRotationSpeed);
        transform.Translate(Vector3.forward * _MissileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDespawning) return;

        if (other.CompareTag("Obstacle") || other.CompareTag("Player"))
        {
            if (other.CompareTag("Player") && other.TryGetComponent<PlayerModel>(out var player))
                player.TakeDamage(_Dmg);

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

        var ps = PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);
        if (ps != null)
            ps.Play();
    }

    private void SpawnExplosion()
    {
        if (_ExplosionPrefab == null) return;

        var ps = PoolManager.Instance.Spawn(_ExplosionPrefab, transform.position, transform.rotation);
        if (ps != null)
            ps.Play();
    }

    private void HandleTrails()
    {
        if (_Trails == null) return;

        for (int i = 0; i < _Trails.Count; i++)
        {
            var ps = _Trails[i];
            if (ps == null) continue;

            ps.transform.parent = null;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    public void OnDespawned()
    {
        _isDespawning = false;

        for (int i = 0; i < _Trails.Count; i++)
        {
            if (_Trails[i] != null)
                _Trails[i].transform.SetParent(transform, false);
        }
    }
}
