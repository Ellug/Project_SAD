using UnityEngine;

[DisallowMultipleComponent]
public class AutoDespawnParticle : MonoBehaviour
{
    ParticleSystem _ps;
    bool _despawnScheduled;

    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        _despawnScheduled = false;

        if (_ps == null)
            return;

        _ps.Play(true);
    }

    void LateUpdate()
    {
        if (_despawnScheduled || _ps == null)
            return;

        // 모든 파티클 + 서브 이미터 종료 확인
        if (!_ps.IsAlive(true))
        {
            _despawnScheduled = true;
            PoolManager.Instance.Despawn(gameObject);
        }
    }

    void OnDisable()
    {
        _despawnScheduled = false;
    }
}
