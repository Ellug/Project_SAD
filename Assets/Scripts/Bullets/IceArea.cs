using UnityEngine;
using System.Collections;

public class IceArea : MonoBehaviour, IPoolable
{
    [Header("냉기 설정")]
    [Tooltip("실제 타격 판정 범위 (지름)")][SerializeField] float _IceAreaRange = 5.0f;
    [Tooltip("냉기 지속 시간")][SerializeField] float _IceAreaLifeTime = 5.0f;
    [Tooltip("냉기 데미지")][SerializeField] float _Dmg = 10.0f;
    [Tooltip("데미지 딜레이")][SerializeField] float _DmgDelay = 0.5f;

    [Header("디버프 설정")]
    [Tooltip("냉기 지속시간")][SerializeField] float _ColdDebuffTime = 2.0f;
    [Tooltip("냉기 데미지")][SerializeField] float _ColdDmg = 5.0f;
    [Tooltip("냉기 틱")][SerializeField] float _TickInterval = 0.5f;

    private GameObject _player;
    private bool _checkDelay = true;
    private Coroutine _delayCoroutine;

    private void OnValidate()
    {
        UpdateVisuals();
    }

    public void Init(GameObject player)
    {
        _player = player;
        _checkDelay = true;
        UpdateVisuals();
        Invoke(nameof(DespawnIceArea), _IceAreaLifeTime);
    }

    private void UpdateVisuals()
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

    private void Update()
    {
        if (_player == null || !_checkDelay) return;

        if (Physics.CheckSphere(transform.position, _IceAreaRange * 0.5f, 1 << _player.layer))
        {
            if (_player.TryGetComponent<PlayerModel>(out var playerModel))
            {
                playerModel.TakeDamage(_Dmg);
                playerModel.ColdDebuff(_ColdDmg, _ColdDebuffTime, _TickInterval);

                _checkDelay = false;
                _delayCoroutine = StartCoroutine(DmgDelayTime());
            }
        }
    }

    private void DespawnIceArea()
    {
        CancelInvoke();
        if (_delayCoroutine != null) StopCoroutine(_delayCoroutine);
        PoolManager.Instance.Despawn(gameObject);
    }

    private IEnumerator DmgDelayTime()
    {
        yield return new WaitForSeconds(_DmgDelay);
        _checkDelay = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _checkDelay ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, _IceAreaRange * 0.5f);
    }

    public void OnSpawned() { }
    public void OnDespawned() { }
}