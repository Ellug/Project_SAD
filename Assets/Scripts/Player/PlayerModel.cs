using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    // 기본값들
    [Header("Status")]
    [SerializeField] private float _maxHp = 50f;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _accelForce = 30f;
    [SerializeField] private float _rotSpeed = 10f;

    [Header("Dodge")]
    [SerializeField] private float _dodgeDuration = 0.1f;
    [SerializeField] private float _dodgeSpeed = 25f;
    [SerializeField] private float _dodgeCoolTime = 5.0f;

    [Header("SPAttack")]
    [SerializeField] private float _specialCoolTime = 3f;

    [Header("AttackSlow")]
    [SerializeField] private float _attackSlowRate = 0.5f;
    [SerializeField] private float _attackSlowDuration = 0.25f;

    // 실제 인게임 적용 값
    private PlayerRuntimeStats _baseStats;
    private PlayerRuntimeStats _runtimeStats;

    // Internal
    private float _curHp;
    private bool _isDodging = false;
    private float _curDodgeTime = 0f;
    private bool _isInvincible = false;
    private bool _isOnAttack = false;
    private bool _isOnSpecialAttack = false;
    private float _curDodgeCoolTime = 0f;

    private float _curAttackSlowTime = 0f;
    private float _attackSpeed = 0f; 
    private float _attackCoolTime = 0f;
    private float _curAttackCoolTime = 0f;
    
    private float _curSpecialCoolTime = 0f;

    // Properties
    public WeaponBase CurrentWeapon { get; private set; }

    public float MaxHp => _runtimeStats.MaxHp;
    public float CurHp => _curHp;
    public float MaxSpeed => _runtimeStats.MaxSpeed;
    public float AccelForce => _runtimeStats.AccelForce;
    public float RotSpeed => _runtimeStats.RotSpeed;
    
    public float AttackSlowRate => _runtimeStats.AttackSlowRate;
    public float AttackSlowDuration => _runtimeStats.AttackSlowDuration;
    public bool IsOnAttack => _isOnAttack;

    public float DodgeSpeed => _runtimeStats.DodgeSpeed;
    public float DodgeDuration => _runtimeStats.DodgeDuration;
    public float DodgeCoolTime => _runtimeStats.DodgeCoolTime;
    public bool IsDodging => _isDodging;

    public float DodgeCooldownCur => _curDodgeCoolTime;
    public float DodgeCooldownRatio => 1f - (_curDodgeCoolTime / DodgeCoolTime);

    public float SpecialCoolTime => _runtimeStats.SpecialCoolTime;
    public float SpecialCooldownCur => _curSpecialCoolTime;
    public float SpecialCooldownRatio => 1f - (_curSpecialCoolTime / SpecialCoolTime);
    public bool CanDodge => !_isDodging && _curDodgeCoolTime <= 0f;
    public bool IsOnSpecialAttack => _isOnSpecialAttack;
    public bool CanSpecialAttack => _curSpecialCoolTime <= 0f;
    public bool CanAttack => _curAttackCoolTime <= 0f;

    

    // 기본 스탯 적용
    void Awake()
    {
        CaptureBaseStats();
        _runtimeStats = _baseStats;
    }

    private void CaptureBaseStats()
    {
        _baseStats = new PlayerRuntimeStats
        {
            MaxHp = _maxHp,
            MaxSpeed = _maxSpeed,
            AccelForce = _accelForce,
            RotSpeed = _rotSpeed,

            DodgeDuration = _dodgeDuration,
            DodgeSpeed = _dodgeSpeed,
            DodgeCoolTime = _dodgeCoolTime,

            SpecialCoolTime = _specialCoolTime,

            AttackSlowRate = _attackSlowRate,
            AttackSlowDuration = _attackSlowDuration
        };
    }

    // 스탯 퍽 -> 런타임 스탯에 적용을 위한 리빌드 메서드
    public void RebuildRuntimeStats(IEnumerable<StatMod> mods, bool resetHpToMax)
    {
        _runtimeStats = _baseStats;
        PerkCalculator.ApplyToPlayer(ref _runtimeStats, mods);

        if (resetHpToMax)
            _curHp = _runtimeStats.MaxHp;
        else
            _curHp = Mathf.Min(_curHp, _runtimeStats.MaxHp);
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        _curHp = _maxHp;
        _curDodgeCoolTime = 0f;
        _curDodgeTime = 0f;
        _isDodging = false;
        _isInvincible = false;
        _curAttackSlowTime = 0f;
        _isOnAttack = false;
        _isOnSpecialAttack = false;
        _curSpecialCoolTime = 0f;
        _curAttackCoolTime = 0f;
    }

    public void StartDodge()
    {
        _isDodging = true;
        _isInvincible = true;
        _curDodgeTime = DodgeDuration;
        _curDodgeCoolTime = DodgeCoolTime;
    }

    public void UpdateDodge(float deltaTime)
    {
        if (!_isDodging) return;

        _curDodgeTime -= deltaTime;

        // 앞 절반 동안만 무적
        if (_curDodgeTime <= DodgeDuration * 0.5f)
            _isInvincible = false;

        // 종료
        if (_curDodgeTime <= 0f)
        {
            _curDodgeTime = 0f;
            _isDodging = false;
            _isInvincible = false;
        }
    }

    public void StopDodge()
    {
        _isDodging = false;
    }
    
    public void StartAttackSlow()
    {
        _isOnAttack = true;
        _curAttackSlowTime = AttackSlowDuration;
    }

    public void UpdateAttackSlow(float deltaTime)
    {
        if(!_isOnAttack) return;

        _curAttackSlowTime -= deltaTime;

        if(_curAttackSlowTime <= 0f)
        {
            _curAttackSlowTime = 0f;
            _isOnAttack = false;
        }
    }
    
    public void StartSpecialAttack()
    {
        if(!CanSpecialAttack) return;
        _curSpecialCoolTime = _runtimeStats.SpecialCoolTime;
    }
    
    public void SetSpecialAttackState(bool value)
    {
        _isOnSpecialAttack = value;
    }

    public void StartAttack()
    {
        if(!CanAttack) return;
        _curAttackCoolTime = _attackCoolTime;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (_curDodgeCoolTime > 0f)
            _curDodgeCoolTime = Mathf.Max(0, _curDodgeCoolTime - deltaTime);

        if (_curSpecialCoolTime > 0f)
            _curSpecialCoolTime = Mathf.Max(0, _curSpecialCoolTime - deltaTime);
        
        if (_curAttackSlowTime > 0f)
            _curAttackSlowTime = Mathf.Max(0f, _curAttackSlowTime - deltaTime);

        if(_curAttackCoolTime > 0f)
            _curAttackCoolTime = Mathf.Max(0f, _curAttackCoolTime - deltaTime);
    }

    public void SetWeapon(WeaponBase weapon)
    {
        CurrentWeapon = weapon;
        _attackSpeed = weapon.WeaponData.attackSpeed;
        _attackCoolTime = 1f / _attackSpeed;
        _curAttackCoolTime = 0f;

        _curSpecialCoolTime = 0f;
    }

    public void TakeDamage(float dmg)
    {
        if (_isInvincible) return; // 무적 판정

        _curHp -= dmg;

        if(_curHp <= 0)
            Die();
    }

    private void Die()
    {
        // Game Over
        GameManager.Instance.PlayerLose();
    }
}
