using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerModel : MonoBehaviour
{
    //WeaponSound
    [SerializeField] private AudioSource _weaponAudioSource;

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

    [Header("Visual Effects")]
    [SerializeField] private MeshRenderer[] _childRenderers; 
    [SerializeField] private Material _burnMaterial; 
    [SerializeField] private Material _coldMaterial;
    [SerializeField] private Material _slowMaterial;

    private HashSet<Material> _activeDebuffs = new HashSet<Material>();
    private Material _baseMaterial;

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

    private float _slowVfxRemain = 0f;

    private bool _isSPFireSoundReady = false;

    // Knockback request (1-shot)
    private bool _hasKbRequest;
    private Vector3 _kbDir;
    private float _kbDistance;
    private float _kbDuration;
    
    //화상 디버프 코루틴
    private Coroutine _burnCoroutine;
    private Coroutine _coldCoroutine;

    // Properties
    public WeaponBase CurrentWeapon { get; private set; }
    public PlayerFinalStats FinalStats => _statsContext.Current;
    public PlayerStatsContext StatsCtx => _statsContext;

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

    [HideInInspector] public float attackImpulse = 0f;

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
        if (_childRenderers.Length > 0) _baseMaterial = _childRenderers[0].sharedMaterial;
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
        _slowVfxRemain = 0f;
        SetSlowVfx(false);
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

    public void SetSlowVfx(bool on)
    {
        UpdateDebuffVisual(_slowMaterial, on);
    }

    public void RefreshSlowVfx(float duration)
    {
        duration = Mathf.Max(0.01f, duration);
        _slowVfxRemain = Mathf.Max(_slowVfxRemain, duration);
        SetSlowVfx(true);
    }

    public void UpdateTimer(float deltaTime)
    {
        if (_curDodgeCoolTime > 0f)
            _curDodgeCoolTime = Mathf.Max(0, _curDodgeCoolTime - deltaTime);

        if (_curSpecialCoolTime > 0f)
            _curSpecialCoolTime = Mathf.Max(0, _curSpecialCoolTime - deltaTime);

        if (_curAttackCoolTime > 0f)
            _curAttackCoolTime = Mathf.Max(0f, _curAttackCoolTime - deltaTime);

        if (_statsContext != null)
        {
            _statsContext.TickBuffs(deltaTime);
            _statsContext.TickDynamicDebuffs(deltaTime);
        }

        if (_slowVfxRemain > 0f)
        {
            _slowVfxRemain -= deltaTime;
            if (_slowVfxRemain <= 0f)
            {
                _slowVfxRemain = 0f;
                SetSlowVfx(false);
            }
        }

        bool canSpecial = _curSpecialCoolTime <= 0f;
        
        if (!_isSPFireSoundReady && canSpecial)
        {
            CurrentWeapon?.NotifySpecialReady();
        }
        _isSPFireSoundReady = canSpecial;
    }

    public void SetWeapon(WeaponBase weapon)
    {
        CurrentWeapon = weapon;

        // Weapon 교체 -> Final 재계산
        _statsContext.SetWeapon(weapon);

        SoundManager.Instance.BindWeapon(weapon, GetComponentInChildren<AudioSource>());

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

    // 디버프 비주얼 업데이트 (마테리얼 추가 / 삭제로 적용)
    private void UpdateDebuffVisual(Material debuffMat, bool shouldAdd)
    {
        if (debuffMat == null) return;

        // 1. 상태 변화 체크 (이미 추가됐거나 이미 없는 경우 실행 안 함)
        bool isChanged = shouldAdd ? _activeDebuffs.Add(debuffMat) : _activeDebuffs.Remove(debuffMat);
        if (!isChanged) return;

        // 2. 새 머티리얼 배열 생성 (기본 + 활성 디버프들)
        Material[] newMats = new Material[_activeDebuffs.Count + 1];
        newMats[0] = _baseMaterial;
        _activeDebuffs.CopyTo(newMats, 1);

        // 3. 모든 렌더러에 한 번에 적용
        foreach (var renderer in _childRenderers)
        {
            if (renderer != null) renderer.materials = newMats;
        }
    }

    public void BurnDebuff(float BurnDmg, float Burnduration, float TickInterval)
    {
        if (_burnCoroutine != null)
            StopCoroutine(_burnCoroutine);

        _burnCoroutine = StartCoroutine(ProcessBurn(BurnDmg, Burnduration, TickInterval));
    }

    public void ColdDebuff(float ColdDmg, float Coldduration, float TickInterval)
    {
        if (_coldCoroutine != null)
            StopCoroutine(_coldCoroutine);

        _coldCoroutine = StartCoroutine(Processcold(ColdDmg, Coldduration, TickInterval));
    }

    private IEnumerator ProcessBurn(float BurnDmg, float Burnduration, float TickInterval)
    {
        UpdateDebuffVisual(_burnMaterial, true);

        float BurnTime = 0;
        while (BurnTime < Burnduration)
        {
            TakeDamage(BurnDmg * TickInterval); 

            yield return new WaitForSeconds(TickInterval);
            BurnTime += TickInterval;
        }

        UpdateDebuffVisual(_burnMaterial, false);
        _burnCoroutine = null; 
    }

    private IEnumerator Processcold(float ColdDmg, float Coldduration, float TickInterval)
    {
        UpdateDebuffVisual(_coldMaterial, true);

        float ColdTime = 0;
        while (ColdTime < Coldduration)
        {
            TakeDamage(ColdDmg * TickInterval);

            yield return new WaitForSeconds(TickInterval);
            ColdTime += TickInterval;
        }

        UpdateDebuffVisual(_coldMaterial, false);
        _coldCoroutine = null;
    }

    private void Die()
    {
        // Game Over
        GameManager.Instance.PlayerLose();
    }

    public void TakeHeal(float heal)
    {
        if (CurHp <= 0) return;

        _curHp += MaxHp * heal;

        if (CurHp > MaxHp)
            _curHp = MaxHp;
    }

    // 넉백 예약
    public void RequestKnockback(Vector3 dir, float distance, float duration)
    {
        dir.y = 0f;
        if (distance <= 0f) return;

        if (dir.sqrMagnitude < 1e-6f) return;
        duration = Mathf.Max(0.01f, duration);

        _hasKbRequest = true;
        _kbDir = dir.normalized;
        _kbDistance = distance;
        _kbDuration = duration;
    }

    // 넉백 소비
    public bool TryConsumeKnockbackRequest(out Vector3 dir, out float distance, out float duration)
    {
        if (!_hasKbRequest)
        {
            dir = default;
            distance = 0f;
            duration = 0f;
            return false;
        }

        _hasKbRequest = false;
        dir = _kbDir;
        distance = _kbDistance;
        duration = _kbDuration;
        return true;
    }

    public void ApplyDebuff(StatMod[] mods, float duration, float vfxDuration = 0f)
    {
        _statsContext.ApplyDebuff(mods, duration);
        
        if (vfxDuration > 0f)
            RefreshSlowVfx(vfxDuration);
    }
}
