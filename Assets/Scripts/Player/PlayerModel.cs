using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private WeaponSound _weaponSound;

    // Base Status
    [Header("HP")]
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
    [SerializeField] private float _attackMinSpeed = 5f;
    [SerializeField] private float _attackSlowRate = 5f;

    // Final Stats Context
    [SerializeField] private PlayerStatsContext _statsContext;

    // Internal
    private float _curHp;

    private bool _isDodging = false;
    private bool _isInvincible = false;
    private float _curDodgeTime = 0f;
    private float _curDodgeCoolTime = 0f;

    private bool _isOnSpecialAttack = false;

    private float _curAttackCoolTime = 0f;
    private float _curSpecialCoolTime = 0f;

    private float _curDebuffSlowTime = 0f;
    private float _debuffSlowRate = 0f;

    // Properties
    public WeaponBase CurrentWeapon { get; private set; }
    public PlayerFinalStats FinalStats => _statsContext.Current;

    public float MaxHp => FinalStats.Player.MaxHp;
    public float CurHp => _curHp;
    public float MaxSpeed => FinalStats.Player.MaxSpeed;
    public float AccelForce => FinalStats.Player.AccelForce;
    public float RotSpeed => FinalStats.Player.RotSpeed;

    public float AttackMinSpeed => FinalStats.Player.AttackMinSpeed;
    public float AttackSlowRate => FinalStats.Player.AttackSlowRate;

    public float DodgeSpeed => FinalStats.Player.DodgeSpeed;
    public float DodgeDuration => FinalStats.Player.DodgeDuration;
    public float DodgeCoolTime => FinalStats.Player.DodgeCoolTime;
    public bool IsDodging => _isDodging;

    public float DodgeCooldownCur => _curDodgeCoolTime;
    public float DodgeCooldownRatio => 1f - (_curDodgeCoolTime / DodgeCoolTime);

    public float SpecialCoolTime => FinalStats.Player.SpecialCoolTime;
    public float SpecialCooldownCur => _curSpecialCoolTime;
    public float SpecialCooldownRatio => 1f - (_curSpecialCoolTime / SpecialCoolTime);
    public bool IsOnSpecialAttack => _isOnSpecialAttack;

    public bool CanSpecialAttack => _curSpecialCoolTime <= 0f;
    public bool CanDodge => !_isDodging && _curDodgeCoolTime <= 0f;
    public bool CanAttack => _curAttackCoolTime <= 0f;
    public float attackImpulse = 0f;

    // 디버프 관련 파이널 스탯츠 or 버프 디버프 매니져로 분리 필요해 보임
    public bool IsInDebuffSlow => _curDebuffSlowTime > 0f;
    public float DebuffSlowRate => _debuffSlowRate;

    // 기본 스탯 적용
    void Awake()
    {
        // 스탯 컨텍스트 없으면 불러오고 추가
        if (_statsContext == null)
            _statsContext = GetComponent<PlayerStatsContext>();

        if (_statsContext == null)
            _statsContext = gameObject.AddComponent<PlayerStatsContext>();

        _statsContext.Bind(this);
    }

    // Base 스탯 스냅샷 생성 (Final 계산용)
    public PlayerRuntimeStats CaptureBaseStatsSnapshot()
    {
        return new PlayerRuntimeStats
        {
            MaxHp = _maxHp,
            MaxSpeed = _maxSpeed,
            AccelForce = _accelForce,
            RotSpeed = _rotSpeed,

            DodgeDuration = _dodgeDuration,
            DodgeSpeed = _dodgeSpeed,
            DodgeCoolTime = _dodgeCoolTime,

            SpecialCoolTime = _specialCoolTime,

            AttackMinSpeed = _attackMinSpeed,
            AttackSlowRate = _attackSlowRate,
        };
    }

    // 스탯 퍽 -> 런타임 스탯에 적용을 위한 리빌드 메서드
    public void RebuildRuntimeStats(IEnumerable<StatMod> mods)
    {
        _statsContext.SetPlayerMods(mods);
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        _curHp = MaxHp;
        _curDodgeCoolTime = 0f;
        _curDodgeTime = 0f;
        _isDodging = false;
        _isInvincible = false;
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

        if (_statsContext != null)
            _statsContext.Trigger(PerkTrigger.OnDodgeUsed);
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
        attackImpulse = AttackSlowRate;
    }

    public void StartSpecialAttack()
    {
        _curSpecialCoolTime = SpecialCoolTime;

        if (_statsContext != null)
            _statsContext.Trigger(PerkTrigger.OnSpecialUsed);
    }

    public void SetSpecialAttackState(bool value)
    {
        _isOnSpecialAttack = value;
    }

    public void StartAttack()
    {
        if (!CanAttack) return;
        _curAttackCoolTime = FinalStats.AttackCoolTime;
    }
    
    public void SlowDebuff(float rate, float duration)
    {
        _debuffSlowRate = Mathf.Clamp01(rate); 
        _curDebuffSlowTime = duration;         
    }

    public void UpdateTimer(float deltaTime)
    {
        if (_curDodgeCoolTime > 0f)
            _curDodgeCoolTime = Mathf.Max(0, _curDodgeCoolTime - deltaTime);

        if (_curSpecialCoolTime > 0f)
            _curSpecialCoolTime = Mathf.Max(0, _curSpecialCoolTime - deltaTime);

        if (_curAttackCoolTime > 0f)
            _curAttackCoolTime = Mathf.Max(0f, _curAttackCoolTime - deltaTime);

        if (_curDebuffSlowTime > 0f)
            _curDebuffSlowTime = Mathf.Max(0f, _curDebuffSlowTime - deltaTime);

        if (_statsContext != null)
            _statsContext.TickBuffs(deltaTime);
    }

    public void SetWeapon(WeaponBase weapon)
    {
        CurrentWeapon = weapon;

        // Weapon 교체 -> Final 재계산
        _statsContext.SetWeapon(weapon);

        _weaponSound.Bind(weapon);

        _curAttackCoolTime = 0f;
        _curSpecialCoolTime = 0f;
    }

    public void TakeDamage(float dmg)
    {
        if (_isInvincible) return; // 무적 판정

        _curHp -= dmg;

        if (_curHp <= 0)
            Die();
    }

    private void Die()
    {
        // Game Over
        GameManager.Instance.PlayerLose();
    }
}
