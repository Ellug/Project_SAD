using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private float _bossMaxHp = 100;
    [SerializeField] private float _changePhaseHpRate = 0.5f;
    private bool _isChangedPhase;

    public float BossMaxHp { get { return _bossMaxHp; }}
    public float BossCurrentHp { get; private set; }

    public event Action _phaseChange;
    public event Action _takeCounterableAttack;

    void Awake()
    {
        _isChangedPhase = false;
        BossCurrentHp = _bossMaxHp;
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 총알에 태그 별도로 할당 바랍니다.
        if (other.CompareTag("PlayerBullet"))
        {
            // TakeDamage(총알 공격력);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(float dmg, bool isCounterable)
    {
        BossCurrentHp -= dmg;
        if (isCounterable) 
        {
            _takeCounterableAttack?.Invoke();
        }
        // 페이즈 전환 조건 검사
        if (_isChangedPhase == false && 
            BossCurrentHp / _bossMaxHp < _changePhaseHpRate)
        {
            _phaseChange?.Invoke();
            _isChangedPhase = true;
        }

        if (BossCurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
