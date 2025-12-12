using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private int _bossMaxHp = 100;
    [SerializeField] private float _changePhaseHpRate = 0.5f;
    private bool _isChangedPhase;

    public int BossCurrentHp { get; private set; }

    public event Action _phaseChange;

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
        }
    }

    private void TakeDamage(int dmg)
    {
        BossCurrentHp -= dmg;
        if (_isChangedPhase == false && 
            (float)BossCurrentHp / _bossMaxHp < _changePhaseHpRate)
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
