using UnityEngine;
using System.Collections;

public class FireArea : MonoBehaviour, IPoolable
{
    protected float _FireAreaRange = 5.0f;
    protected float _FireAreaLifeTime = 5.0f;
    protected float _Dmg = 10.0f;
    protected float _DmgDelay = 0.5f;

    protected float _BurnDebuffTime = 2.0f;
    protected float _BurnDmg = 5.0f;
    protected float _TickInterval = 0.5f;

    protected GameObject _player;
    protected bool _checkDelay = true;
    protected Coroutine _delayCoroutine;

    public void Init(GameObject player, float dmg, float range, float lifeTime, float burnDmg, float burnTime, float burnTick)
    {
        _player = player;
        _Dmg = dmg;
        _FireAreaRange = range;
        _FireAreaLifeTime = lifeTime;
        _BurnDmg = burnDmg;
        _BurnDebuffTime = burnTime;
        _TickInterval = burnTick;

        _checkDelay = true;
        UpdateVisuals();
        Invoke(nameof(DespawnFireArea), _FireAreaLifeTime);
    }

    protected virtual void Update()
    {
        if (_player == null || !_checkDelay) return;

        if (Physics.CheckSphere(transform.position, _FireAreaRange * 0.5f, 1 << _player.layer))
        {
            if (_player.TryGetComponent<PlayerModel>(out var playerModel))
            {
                playerModel.TakeDamage(_Dmg);
                playerModel.BurnDebuff(_BurnDmg, _BurnDebuffTime, _TickInterval);

                _checkDelay = false;
                _delayCoroutine = StartCoroutine(DmgDelayTime());
            }
        }
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
            float finalScale = (_FireAreaRange / 5.0f) * multiplier;
            child.localScale = new Vector3(finalScale, finalScale, finalScale);
        }
    }

    protected virtual void DespawnFireArea()
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

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }
}