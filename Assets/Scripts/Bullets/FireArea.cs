using UnityEngine;
using System.Collections;

public class FireArea : MonoBehaviour, IPoolable
{
    [Header("화염장판 설정")]
    [Tooltip("실제 타격 판정 범위 (지름)")][SerializeField] float _FireAreaRange = 5.0f;
    [Tooltip("화염장판 지속 시간")][SerializeField] float _FireAreaLifeTime = 5.0f;
    [Tooltip("화염장판 데미지")][SerializeField] float _Dmg = 10.0f;
    [Tooltip("데미지 딜레이")][SerializeField] float _DmgDelay = 0.5f;

    [Header("디버프 설정")]
    [Tooltip("화상 지속시간")][SerializeField] float _BurnDebuffTime = 2.0f;
    [Tooltip("화상 데미지")][SerializeField] float _BurnDmg = 5.0f;
    [Tooltip("화상 틱")][SerializeField] float _TickInterval = 0.5f;

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
        Invoke(nameof(DespawnFireArea), _FireAreaLifeTime);
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

            float finalScale = (_FireAreaRange / 5.0f) * multiplier;

            child.localScale = new Vector3(finalScale, finalScale, finalScale);
        }
    }

    private void Update()
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

    private void DespawnFireArea()
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
        Gizmos.DrawWireSphere(transform.position, _FireAreaRange * 0.5f);
    }

    public void OnSpawned() { }
    public void OnDespawned() { }
}