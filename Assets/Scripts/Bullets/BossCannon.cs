using System.Collections.Generic;
using UnityEngine;

public class BossCannon : BulletBase
{
    [Header("VFX Prefabs")]
    [Tooltip("총구 화염 효과")] public GameObject _MuzzlePrefab;
    [Tooltip("폭발 파티클 프리팹")] public GameObject _ExplosionParticle;
    [Tooltip("총알에 붙어있는 잔상(Trail) 리스트")] public List<GameObject> _Trails;
    void Start()
    {

        if (_MuzzlePrefab != null)
        {
            GameObject muzzleVFX = Instantiate(_MuzzlePrefab, transform.position, transform.rotation);

            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else if (muzzleVFX.transform.childCount > 0)
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                if (psChild != null) Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }
    void OnTriggerEnter(Collider other)
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
        var ps = instance.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            ps.Play();
        }
        else if (instance.transform.childCount > 0)
        {
            var childPS = instance.transform.GetChild(0).GetComponent<ParticleSystem>();
            if (childPS != null)
                Destroy(instance, childPS.main.duration + childPS.main.startLifetime.constantMax);
        }
    }
}