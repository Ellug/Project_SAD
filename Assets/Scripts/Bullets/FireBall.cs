using System.Collections.Generic;
using UnityEngine;

public class FireBall : BulletBase
{
    [Header("VFX Prefabs")]
    [SerializeField] protected ParticleSystem _MuzzlePrefab;
    [SerializeField] protected ParticleSystem _ExplosionParticle;
    [SerializeField] protected List<GameObject> _Trails;

    [Header("데칼 & 장판")]
    [SerializeField] protected BurnDecal _BurnDecalPrefab;
    [SerializeField] protected FireArea _FireAreaPrefab;

    protected Vector3 _targetPos;
    protected bool _isArrived = false;
    protected GameObject _player;

    protected float _areaDmg;
    protected float _areaRange;
    protected float _areaLifeTime;
    protected float _burnDmg;
    protected float _burnTime;
    protected float _burnTickInterval;

    public void SetFireBallStats(GameObject player, Vector3 targetPoint, float areaDmg, float areaRange, float areaLifeTime, float burnDmg, float burnTime, float burnTick)
    {
        _player = player;
        _targetPos = targetPoint;
        _areaDmg = areaDmg;
        _areaRange = areaRange;
        _areaLifeTime = areaLifeTime;
        _burnDmg = burnDmg;
        _burnTime = burnTime;
        _burnTickInterval = burnTick;

        _isArrived = false;
        transform.LookAt(_targetPos);
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
                if (trail.TryGetComponent<ParticleSystem>(out var ps)) ps.Play();
            }
        }
    }

    protected override void Update()
    {
        if (_isArrived) return;
        base.Update();

        if (Vector3.Distance(transform.position, _targetPos) < 0.5f)
        {
            ExplodeAtTarget();
        }
    }

    protected virtual void ExplodeAtTarget()
    {
        if (_isArrived) return;
        _isArrived = true;

        if (_BurnDecalPrefab != null)
            PoolManager.Instance.Spawn(_BurnDecalPrefab, _targetPos, Quaternion.identity);

        if (_FireAreaPrefab != null)
        {
            FireArea area = PoolManager.Instance.Spawn(_FireAreaPrefab, _targetPos, Quaternion.identity);
            if (area != null)
            {
                area.Init(_player, _areaDmg, _areaRange, _areaLifeTime, _burnDmg, _burnTime, _burnTickInterval);
            }
        }

        HandleTrails();
        SpawnExplosion();
        Despawn();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _targetPos = transform.position;
            ExplodeAtTarget();
        }
    }

    protected virtual void HandleTrails()
    {
        if (_Trails == null) return;
        foreach (var trail in _Trails)
        {
            if (trail == null) continue;
            trail.transform.parent = null;
            if (trail.TryGetComponent<ParticleSystem>(out var ps))
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    protected virtual void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;
        ParticleSystem instance = PoolManager.Instance.Spawn(_ExplosionParticle, transform.position, transform.rotation);
        if (instance != null) instance.Play();
    }
}