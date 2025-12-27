using System;
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private float _bossMaxHp = 100;
    [SerializeField] private float _changePhaseHpRate = 0.5f;
    private bool _isChangedPhase;

    public float BossMaxHp { get { return _bossMaxHp; }}
    public float BossCurrentHp { get; private set; }

    // Burn
    private float _burnTickInterval = 0.5f;
    private float _burnTickAcc;
    private float _burnRemain;
    private float _burnDps;

    public event Action _phaseChange;
    public event Action _takeCounterableAttack;

    void Awake()
    {
        _isChangedPhase = false;
        BossCurrentHp = _bossMaxHp;
    }
    void Update()
    {
        TickBurn(Time.deltaTime);
    }

    public void TakeDamage(float dmg, bool isCounterable)
    {
        BossCurrentHp -= dmg;

        if (isCounterable)
            _takeCounterableAttack?.Invoke();

        // 페이즈 전환 조건 검사
        if (_isChangedPhase == false && 
            BossCurrentHp / _bossMaxHp < _changePhaseHpRate)
        {
            _phaseChange?.Invoke();
            _isChangedPhase = true;
        }

        if (BossCurrentHp <= 0f)
        {
            BossCurrentHp = 0f;
            StartCoroutine(DieProcess());
        }
    }

    private IEnumerator DieProcess()
    {
        yield return null;
        Die();
    }

    private void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.PlayerWin();
    }

    public void ApplyBurn(float dps, float duration)
    {
        if (BossCurrentHp <= 0f) return;
        if (dps <= 0f) return;
        if (duration <= 0f) return;

        // 서로 다른 burn 수치가 들어왔을 때 높은 쪽을 선택
        _burnDps = Mathf.Max(_burnDps, dps);
        _burnRemain = Mathf.Max(_burnRemain, duration);
    }

    private void TickBurn(float dt)
    {
        if (_burnRemain <= 0f) return;
        if (BossCurrentHp <= 0f) return;

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
