using UnityEngine;
using System.Collections;

public class Flamethrower : MonoBehaviour, IPoolable
{
    protected float _Distance;
    protected float _StartRadius;
    protected float _EndRadius;
    protected float _Density;
    protected float _LifeTime;
    protected float _Dmg;
    protected float _DmgDelay = 0.5f;

    protected float _BurnDebuffTime;
    protected float _BurnDmg;
    protected float _TickInterval;

    protected GameObject _player;
    protected bool _checkDelay = true;
    protected Coroutine _delayCoroutine;

    public void Init(GameObject player, float dist, float sRad, float eRad, float dens, float life, float dmg, float bDmg, float bTime, float bTick)
    {
        _player = player;
        _Distance = dist;
        _StartRadius = sRad;
        _EndRadius = eRad;
        _Density = dens;
        _LifeTime = life;
        _Dmg = dmg;
        _BurnDmg = bDmg;
        _BurnDebuffTime = bTime;
        _TickInterval = bTick;

        _checkDelay = true;
        Invoke(nameof(DespawnArea), _LifeTime + 0.1f);
    }

    protected virtual void Update()
    {
        if (_player == null || !_checkDelay) return;

        if (CheckConeCollision())
        {
            if (_player.TryGetComponent<PlayerModel>(out var playerModel))
            {
                playerModel.TakeDamage(_Dmg);
                playerModel.BurnDebuff(_BurnDmg, _BurnDebuffTime, _TickInterval);

                _checkDelay = false;
                if (gameObject.activeInHierarchy)
                    _delayCoroutine = StartCoroutine(DmgDelayTime());
            }
        }
    }

    protected virtual bool CheckConeCollision()
    {
        float currentDist = 0;
        int safetyLimit = 0;

        while (currentDist <= _Distance && safetyLimit < 50)
        {
            safetyLimit++;
            float ratio = currentDist / _Distance;
            float currentRadius = Mathf.Lerp(_StartRadius, _EndRadius, ratio);
            Vector3 checkPos = transform.position + (transform.forward * currentDist);

            if (Physics.CheckSphere(checkPos, currentRadius, 1 << _player.layer)) return true;

            currentDist += Mathf.Max(currentRadius * _Density, 0.3f);
        }
        return false;
    }

    protected virtual void DespawnArea()
    {
        CancelInvoke();
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        if (PoolManager.Instance != null)
            PoolManager.Instance.Despawn(gameObject);
    }

    protected virtual IEnumerator DmgDelayTime()
    {
        yield return new WaitForSeconds(_DmgDelay);
        _checkDelay = true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = (Application.isPlaying && !_checkDelay) ? Color.green : Color.red;

        float currentDist = 0;
        int safetyLimit = 0;

        while (currentDist <= _Distance && safetyLimit < 50)
        {
            safetyLimit++;
            float ratio = currentDist / _Distance;
            float currentRadius = Mathf.Lerp(_StartRadius, _EndRadius, ratio);
            Vector3 checkPos = transform.position + (transform.forward * currentDist);

            Gizmos.DrawWireSphere(checkPos, currentRadius);

            currentDist += Mathf.Max(currentRadius * _Density, 0.3f);
        }
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned()
    {
        _checkDelay = true;
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
    }
}