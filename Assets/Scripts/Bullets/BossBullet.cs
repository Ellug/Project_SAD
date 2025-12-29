using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : BulletBase
{
    [Header("Hit Area")]
    [Tooltip("히트 판정 길이")][SerializeField] float HitAreaLength = 1.5f;
    [Tooltip("히트 판정 반지름")][SerializeField] float _HitAreaRadius = 0.3f;
    [Tooltip("플레이어 레이어")][SerializeField] LayerMask _PlayerLayer;

    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public ParticleSystem _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public ParticleSystem _ExplosionParticle;
    [Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] public List<GameObject> _Trails;
    private bool _hit;

    void Start()
    {
        if (_MuzzlePrefab != null)
        {
            ParticleSystem muzzleVFX =
                PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);

            if (muzzleVFX != null &&
                muzzleVFX.TryGetComponent<ParticleSystem>(out var ps))
            {
                ps.Play();
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckHit();
    }

    void CheckHit()
    {
        if (_hit) return;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * HitAreaLength;

        if (Physics.CheckCapsule(start, end, _HitAreaRadius, _PlayerLayer))
        {
            _hit = true;

            Collider[] hits = Physics.OverlapCapsule(start, end, _HitAreaRadius, _PlayerLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_dmg);
                    break;
                }
            }

            HandleTrails();
            SpawnExplosion();
            Despawn();
        }
    }

    void HandleTrails()
    {
        if (_Trails == null || _Trails.Count == 0) return;

        for (int i = 0; i < _Trails.Count; i++)
        {
            if (_Trails[i] == null) continue;

            _Trails[i].transform.parent = null;

            if (_Trails[i].TryGetComponent<ParticleSystem>(out var ps))
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;

        ParticleSystem instance =
            PoolManager.Instance.Spawn(_ExplosionParticle, transform.position, transform.rotation);

        if (instance != null &&
            instance.TryGetComponent<ParticleSystem>(out var ps))
        {
            ps.Play();
        }
    }

    public override void OnSpawned()
    {
        base.OnSpawned();
        _hit = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * HitAreaLength;

        Gizmos.DrawWireSphere(start, _HitAreaRadius);
        Gizmos.DrawWireSphere(end, _HitAreaRadius);
        Gizmos.DrawLine(start + Vector3.up * _HitAreaRadius, end + Vector3.up * _HitAreaRadius);
        Gizmos.DrawLine(start - Vector3.up * _HitAreaRadius, end - Vector3.up * _HitAreaRadius);
        Gizmos.DrawLine(start + Vector3.right * _HitAreaRadius, end + Vector3.right * _HitAreaRadius);
        Gizmos.DrawLine(start - Vector3.right * _HitAreaRadius, end - Vector3.right * _HitAreaRadius);
    }
}
