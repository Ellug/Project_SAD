using System.Collections.Generic;
using UnityEngine;

public class FireBall : BulletBase, IPoolable
{
    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public ParticleSystem _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public ParticleSystem _ExplosionParticle;
    [Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] public List<GameObject> _Trails;

    [Header("데칼 & 장판")]
    [Tooltip("그을음 데칼 프리팹")] public BurnDecal _BurnDecalPrefab;
    [Tooltip("화염 장판 프리팹")] public FireArea _FireAreaPrefab;

    private Vector3 _targetPos;
    private bool _isArrived = false;
    private GameObject _player;

    public void Init(GameObject player)
    {
        _player = player;
    }

    public void setTarget(Vector3 targetPoint)
    {
        _targetPos = targetPoint;
        _isArrived = false;
        transform.LookAt(_targetPos);
    }

    public void Start()
    {
        if (_MuzzlePrefab != null)
        {
            ParticleSystem muzzle =
                PoolManager.Instance.Spawn(_MuzzlePrefab, transform.position, transform.rotation);

            if (muzzle != null &&
                muzzle.TryGetComponent<ParticleSystem>(out var ps))
            {
                ps.Play();
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

    private void ExplodeAtTarget()
    {
        if (_isArrived) return;
        _isArrived = true;

        if (_BurnDecalPrefab != null)
            PoolManager.Instance.Spawn(_BurnDecalPrefab, _targetPos, Quaternion.identity);

        if (_FireAreaPrefab != null)
        {
            FireArea area =
                PoolManager.Instance.Spawn(_FireAreaPrefab, _targetPos, Quaternion.identity);
            if (area != null)
            {
                area.Init(_player);
            }
        }

        HandleTrails();
        SpawnExplosion();
        Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _targetPos = transform.position;
            ExplodeAtTarget();
        }
    }

    private void HandleTrails()
    {
        if (_Trails == null) return;

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

    private void SpawnExplosion()
    {
        if (_ExplosionParticle == null) return;

        ParticleSystem instance = PoolManager.Instance.Spawn(_ExplosionParticle, transform.position, transform.rotation);

        if (instance != null &&
            instance.TryGetComponent<ParticleSystem>(out var ps))
        {
            ps.Play();
        }
    }
}
