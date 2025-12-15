using UnityEngine;

public class AreaOfEffectController : MonoBehaviour
{
    private int _damage;
    private float _tickInterval;
    private float _elapsedTime;

    public void Init(int damage, float tickInterval)
    {
        _damage = damage;
        _tickInterval = tickInterval;
        _elapsedTime = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO : 플레이어가 장판에 닿는 순간 데미지를 바로 1회 줄것인가?
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _tickInterval) 
            {
                // TODO : 플레이어에게 피해를 주는 로직 추가
                Debug.Log($"플레이어 장판 피해 {_damage} 입음");
                _elapsedTime = 0f;
            }
        }
    }

    // 장판에서 벗어나면 경과 시간을 초기화 해야함.
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _elapsedTime = 0f;
        }
    }
}
