using DamageNumbersPro;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    [Header("건드리지 말 것")]
    [SerializeField] private DamageNumber _dmgFont;
    
    public float MaxHp = 10000f;

    // Burn
    private float _burnTickInterval = 0.5f;
    private float _burnTickAcc;
    private float _burnRemain;
    private float _burnDps;

    void Update()
    {
        TickBurn(Time.deltaTime);
    }

    public void TakeDamage(float dmg, bool isCounterable)
    {
        _dmgFont.Spawn(transform.position, dmg);
    }

    public void ApplyBurn(float dps, float duration)
    {
        if (dps <= 0f) return;
        if (duration <= 0f) return;

        // 서로 다른 burn 수치가 들어왔을 때 높은 쪽을 선택
        _burnDps = Mathf.Max(_burnDps, dps);
        _burnRemain = Mathf.Max(_burnRemain, duration);
    }

    private void TickBurn(float dt)
    {
        if (_burnRemain <= 0f) return;

        _burnRemain -= dt;
        _burnTickAcc += dt;

        while (_burnTickAcc >= _burnTickInterval && _burnRemain > 0f)
        {
            _burnTickAcc -= _burnTickInterval;

            float tickDmg = _burnDps * _burnTickInterval;
            TakeDamage(tickDmg, isCounterable: false);
            // 여기서 애니메이션이나 렌더러 색 변경해서 시각 표현도 같이 해주면 좋을듯?
        }

        if (_burnRemain <= 0f)
        {
            _burnRemain = 0f;
            _burnDps = 0f;
            _burnTickAcc = 0f;
        }
    }
}
