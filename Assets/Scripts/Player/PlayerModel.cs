using UnityEngine;

public class PlayerModel : MonoBehaviour
{
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

    [Header("Special Attack")]
    [SerializeField] private float _specialCoolTime = 4.0f;

    [Header("AttackSlow")]
    [SerializeField] private float _onAttackSlowRate = 0.5f;
    [SerializeField] private float _onAttackSlowDuration = 0.25f;

    // Internal
    private float _curHp;
    private bool _isDodging = false;
    private float _curDodgeTime = 0f;
    private bool _isInvincible = false;
    private bool _isOnAttack = false;
    private float _curDodgeCoolTime = 0f;

    private float _curSpecialCoolTime = 0f;

    private float _curAttackSlowTime = 0f;

    // Properties
    public WeaponController CurrentWeapon { get; private set; }
    public Weapon CurrentWeaponType { get; private set; }
    public WeaponData CurrentWeaponData { get; private set; }

    public float MaxHp => _maxHp;
    public float CurHp => _curHp;
    public float MaxSpeed => _maxSpeed;
    public float AccelForce => _accelForce;
    public float RotSpeed => _rotSpeed;
    
    public float OnAttackSlowRate => _onAttackSlowRate;
    public bool IsOnAttack => _isOnAttack;

    public float DodgeSpeed => _dodgeSpeed;
    public float DodgeDuration => _dodgeDuration;
    public float DodgeCoolTime => _dodgeCoolTime;
    public bool IsDodging => _isDodging;

    public float DodgeCooldownCur => _curDodgeCoolTime;
    public float DodgeCooldownRatio => 1f - (_curDodgeCoolTime / _dodgeCoolTime);

    public float SpecialCooldownCur => _curSpecialCoolTime;
    public float SpecialCooldownRatio => 1f - (_curSpecialCoolTime / _specialCoolTime);

    public bool CanDodge => !_isDodging && _curDodgeCoolTime <= 0f;
    public bool CanSpecialAttack => _curSpecialCoolTime <= 0f;

    void Start()
    {
        Init();

        CurrentWeapon = GetComponentInChildren<WeaponController>();

        var wm = WeaponManager.Instance;

        Debug.Log($"[PlayerModel] WeaponManager : {wm.CurrentWeapon}, {wm.CurrentWeaponData}");

        CurrentWeaponType = wm.CurrentWeapon;
        CurrentWeaponData = wm.CurrentWeaponData;

        CurrentWeapon.Init(CurrentWeaponData);
    }


    public void Init()
    {
        _curHp = _maxHp;
        _curDodgeCoolTime = 0f;
        _curSpecialCoolTime = 0f;
        _curDodgeTime = 0f;
        _isDodging = false;
        _isInvincible = false;
        _curAttackSlowTime = 0f;
        _isOnAttack = false;
    }

    public void StartDodge()
    {
        _isDodging = true;
        _isInvincible = true;
        _curDodgeTime = _dodgeDuration;
        _curDodgeCoolTime = _dodgeCoolTime;
    }

    public void UpdateDodge(float deltaTime)
    {
        if (!_isDodging) return;

        _curDodgeTime -= deltaTime;

        // 앞 절반 동안만 무적
        if (_curDodgeTime <= _dodgeDuration * 0.5f)
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
        _curAttackSlowTime = _onAttackSlowDuration;
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

    public void StartSpecial()
    {
        _curSpecialCoolTime = _specialCoolTime;
    }

    public void UpdateTimer(float deltaTime)
    {
        if (_curDodgeCoolTime > 0f)
            _curDodgeCoolTime = Mathf.Max(0, _curDodgeCoolTime - deltaTime);

        if (_curSpecialCoolTime > 0f)
            _curSpecialCoolTime = Mathf.Max(0, _curSpecialCoolTime - deltaTime);
        
        if (_curAttackSlowTime > 0f)
            _curAttackSlowTime = Mathf.Max(0f, _curAttackSlowTime - deltaTime);
    }

    public void SetWeapon(WeaponController weapon)
    {
        CurrentWeapon = weapon;
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
