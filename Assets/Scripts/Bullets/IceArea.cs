using UnityEngine;
using System.Collections;

public class IceArea : MonoBehaviour, IPoolable
{
    protected float _IceAreaRange;
    protected float _IceAreaLifeTime;
    protected float _Dmg;
    protected float _DmgDelay;

    protected float _ColdDebuffTime;
    protected float _ColdDmg;
    protected float _TickInterval;

    protected GameObject _player;
    protected bool _checkDelay = true;
    protected Coroutine _delayCoroutine;

    private void OnValidate()
    {
        UpdateVisuals();
    }

    public void Init(GameObject player, float range, float life, float dmg, float delay, float cDmg, float cTime, float cTick)
    {
        _player = player;
        _IceAreaRange = range;
        _IceAreaLifeTime = life;
        _Dmg = dmg;
        _DmgDelay = delay;
        _ColdDmg = cDmg;
        _ColdDebuffTime = cTime;
        _TickInterval = cTick;

        _checkDelay = true;
        UpdateVisuals();
        Invoke(nameof(DespawnIceArea), _IceAreaLifeTime);
    }

    protected virtual void UpdateVisuals()
    {
        if (transform.childCount == 0) return;
        Transform bottom = transform.GetChild(0);
        if (bottom == null) return;

        for (int i = 0; i < bottom.childCount; i++)
        {
            Transform child = bottom.GetChild(i);
            float multiplier = child.name.Contains("Aura") ? 1.6f : 1.0f;
            float finalScale = (_IceAreaRange / 5.0f) * multiplier;
            child.localScale = new Vector3(finalScale, finalScale, finalScale);
        }
    }

    protected virtual void Update()
    {
        if (_player == null || !_checkDelay) return;

        if (Physics.CheckSphere(transform.position, _IceAreaRange * 0.5f, 1 << _player.layer))
        {
            if (_player.TryGetComponent<PlayerModel>(out var playerModel))
            {
                playerModel.TakeDamage(_Dmg);
                playerModel.ColdDebuff(_ColdDmg, _ColdDebuffTime, _TickInterval);

                _checkDelay = false;
                if (gameObject.activeInHierarchy)
                    _delayCoroutine = StartCoroutine(DmgDelayTime());
            }
        }
    }

    protected virtual void DespawnIceArea()
    {
        CancelInvoke();
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        PoolManager.Instance.Despawn(gameObject);
    }

    protected virtual IEnumerator DmgDelayTime()
    {
        yield return new WaitForSeconds(_DmgDelay);
        _checkDelay = true;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = _checkDelay ? Color.blue : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _IceAreaRange * 0.5f);
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned()
    {
        _checkDelay = true;
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
    }
}