using UnityEngine;
using System.Collections;

public class FlamethrowerArea : MonoBehaviour, IPoolable
{
    [Header("화염 방사 설정")]
    [Tooltip("화염방사 총 길이")][SerializeField] float _Distance = 16.0f;
    [Tooltip("시작 지점 반지름")][SerializeField] float _StartRadius = 0.5f;
    [Tooltip("끝 지점 반지름")][SerializeField] float _EndRadius = 2.0f;
    [Tooltip("구체 간 밀도 (낮을수록 촘촘함)")][SerializeField] float _Density = 0.5f;
    [Tooltip("지속 시간")][SerializeField] float _LifeTime = 5.0f;
    [Tooltip("데미지")][SerializeField] float _Dmg = 10.0f;
    [Tooltip("데미지 딜레이")][SerializeField] float _DmgDelay = 0.5f;

    [Header("디버프 설정")]
    [Tooltip("화상 지속시간")][SerializeField] float _BurnDebuffTime = 2.0f;
    [Tooltip("화상 데미지")][SerializeField] float _BurnDmg = 5.0f;
    [Tooltip("화상 틱")][SerializeField] float _TickInterval = 0.5f;

    private GameObject _player;
    private bool _checkDelay = true;
    private Coroutine _delayCoroutine;

    public void Init(GameObject player)
    {
        _player = player;
        _checkDelay = true;
        Invoke(nameof(DespawnArea), _LifeTime);
    }

    private void Update()
    {
        if (_player == null || !_checkDelay) return;

        if (CheckConeCollision())
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

    private bool CheckConeCollision()
    {
        float currentDist = 0;
        while (currentDist <= _Distance)
        {
            float ratio = currentDist / _Distance;
            float currentRadius = Mathf.Lerp(_StartRadius, _EndRadius, ratio);
            Vector3 checkPos = transform.position + (transform.forward * currentDist);

            if (Physics.CheckSphere(checkPos, currentRadius, 1 << _player.layer))
            {
                return true;
            }

            currentDist += currentRadius * _Density;
        }
        return false;
    }

    private void DespawnArea()
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
        if (_player == null) return;

        Gizmos.color = _checkDelay ? Color.red : Color.green;
        float currentDist = 0;
        while (currentDist <= _Distance)
        {
            float ratio = currentDist / _Distance;
            float currentRadius = Mathf.Lerp(_StartRadius, _EndRadius, ratio);
            Vector3 checkPos = transform.position + (transform.forward * currentDist);

            Gizmos.DrawWireSphere(checkPos, currentRadius);

            currentDist += currentRadius * _Density;
        }
    }

    public void OnSpawned() { }
    public void OnDespawned() { }
}