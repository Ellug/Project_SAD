using System.Collections.Generic;
using UnityEngine;

public class FireBall : BulletBase, IPoolable
{
    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public GameObject _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public GameObject _ExplosionParticle;
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

    public void setTarget(Transform targetPoint)
    {
        _targetPos = targetPoint.position;
        _isArrived = false;
        transform.LookAt(_targetPos);
    }

    public void Start()
    {
        if (_MuzzlePrefab != null)
        {
            GameObject muzzle = Instantiate(_MuzzlePrefab, transform.position, transform.rotation);
            var ps = muzzle.GetComponent<ParticleSystem>() ?? muzzle.GetComponentInChildren<ParticleSystem>();
            if (ps != null) Destroy(muzzle, ps.main.duration);
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
            FireArea area = PoolManager.Instance.Spawn(_FireAreaPrefab, _targetPos, Quaternion.identity);
            if (area != null)
            {
                // 장판의 Init 호출하여 플레이어 정보 전달
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