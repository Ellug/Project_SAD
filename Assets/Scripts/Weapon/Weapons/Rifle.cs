using UnityEngine;

public enum RiflePerkMode
{
    None,
    NoBrain,
    Minigun
}

public class Rifle : WeaponBase
{

    [Header("NoBrain")]
    [SerializeField] private float _stackInterval = 1f;
    [SerializeField] private int _maxStacks = 5;
    [SerializeField] private float _breakInterval = 0.5f;
    [SerializeField] private float _attackMulPerStack = 0.05f;
    [SerializeField] private float _atkSpdMulPerStack = 0.05f;

    private int _dynOwnerId;

    private bool _isHold;

    // 현재 모드 = 노드에서 결정
    private RiflePerkMode _cachedMode = RiflePerkMode.None;

    private RiflePerkMode CurrentMode
    {
        get
        {
            if (_statsContext == null) return RiflePerkMode.None;
            return (RiflePerkMode)_statsContext.Current.Weapon.RifleMode; // 0 1 2
        }
    }

    // NoBrain state
    private bool _hasFiredWhileHolding;
    private float _timeSinceLastFired = 999f;
    private float _stackAcc;
    private int _stacks;
    private int _appliedStacks;

    // Minigun state
    private float _spinTarget = 2f;
    private float _spinTime;
    private bool _spunUp;

    protected override void Awake()
    {
        base.Awake();
        _dynOwnerId = GetInstanceID(); // int 키
    }

    private void OnDisable()
    {
        Clear();
    }

    private void Update()
    {
        if (!_isHold) return;

        //  홀드 중 무기 변경시 안전처리
        var mode = CurrentMode;
        if (mode != _cachedMode)
        {
            ClearDynamic();
            StartHoldState(mode);
        }

        TickHold(Time.deltaTime, mode);
    }

    public void SetAttackHold(bool isHold)
    {
        if (_isHold == isHold) return;
        _isHold = isHold;

        if (_isHold)
        {
            var mode = CurrentMode;
            StartHoldState(mode);
        }
        else
        {
            ResetHoldState(_cachedMode);
        }
    }

    private void StartHoldState(RiflePerkMode mode)
    {
        _cachedMode = mode;

        switch (mode)
        {
            case RiflePerkMode.NoBrain:
                _hasFiredWhileHolding = false;
                _timeSinceLastFired = 999f;
                _stackAcc = 0f;
                _stacks = 0;
                _appliedStacks = 0;
                ApplyStacksMods(0);
                break;

            case RiflePerkMode.Minigun:
                _spinTime = 0f;
                _spunUp = false;
                // 미니건 능력치는 노드에서 상시 적용(여기서 스탯 적용 X)
                break;

            default:
                ClearDynamic();
                break;
        }
    }

    private void ResetHoldState(RiflePerkMode mode)
    {
        switch (mode)
        {
            case RiflePerkMode.NoBrain:
                _hasFiredWhileHolding = false;
                _timeSinceLastFired = 999f;
                _stackAcc = 0f;
                _stacks = 0;
                _appliedStacks = 0;
                ApplyStacksMods(0);
                break;

            case RiflePerkMode.Minigun:
                _spinTime = 0f;
                _spunUp = false;
                break;

            default:
                break;
        }

        _cachedMode = RiflePerkMode.None;
    }

    private void TickHold(float dt, RiflePerkMode mode)
    {
        if (_statsContext == null) return;

        if (mode == RiflePerkMode.NoBrain)
            TickNoBrain(dt);
        else if (mode == RiflePerkMode.Minigun)
            TickMinigun(dt);
    }

    // 무지성 난사 틱 계산
    private void TickNoBrain(float dt)
    {
        _timeSinceLastFired += dt;
        if (!_hasFiredWhileHolding) return;

        if (_timeSinceLastFired > _breakInterval)
        {
            if (_stacks != 0)
            {
                _stacks = 0;
                _stackAcc = 0f;
                ApplyStacksMods(0);
            }
            return;
        }

        _stackAcc += dt;
        while (_stackAcc >= _stackInterval && _stacks < _maxStacks)
        {
            _stackAcc -= _stackInterval;
            _stacks++;
        }

        if (_stacks != _appliedStacks)
            ApplyStacksMods(_stacks);
    }

    // 미니건 틱 계산 -> 2초 누르고 있어야 발사 시작
    private void TickMinigun(float dt)
    {
        if (_spunUp) return;

        _spinTime += dt;
        if (_spinTime >= _spinTarget)
            _spunUp = true;
    }

    // 공격 시도
    public override bool TryAttack()
    {
        var mode = CurrentMode;

        if (mode == RiflePerkMode.Minigun && !_spunUp)
            return false;

        FireRifleSpread(false); // 랜덤 오차각

        _statsContext?.NotifyAttackSlow();

        if (mode == RiflePerkMode.NoBrain)
        {
            _hasFiredWhileHolding = true;
            _timeSinceLastFired = 0f;
        }

        return true;
    }

    // 탄퍼짐 계산
    private void FireRifleSpread(bool isSpecial)
    {
        if (_statsContext == null) return;

        var stats = _statsContext.Current.Weapon;

        float totalAngle = isSpecial ? stats.SpecialProjectileAngle : stats.ProjectileAngle;
        float halfAngle = totalAngle * 0.5f;

        Vector3 baseDir = transform.forward;
        baseDir.y = 0f;
        baseDir.Normalize();

        Vector3 spawnPos = transform.position + baseDir * 0.5f;

        float angle = Random.Range(-halfAngle, halfAngle);
        Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;

        SpawnBullet(spawnPos, dir, stats, isSpecial);
    }


    // NoBrain 스택
    private void ApplyStacksMods(int stacks)
    {
        _appliedStacks = stacks;
        if (_statsContext == null) return;

        if (stacks <= 0)
        {
            _statsContext.SetDynamicMods(_dynOwnerId, null);
            return;
        }

        float atkMul = _attackMulPerStack * stacks;
        float spdMul = _atkSpdMulPerStack * stacks;

        StatMod[] mods =
        {
            new() { stat = StatId.Weapon_Attack,      op = ModOp.Mul, value = atkMul },
            new() { stat = StatId.Weapon_AttackSpeed, op = ModOp.Mul, value = spdMul },
        };

        _statsContext.SetDynamicMods(_dynOwnerId, mods);
    }

    private void ClearDynamic()
    {
        _hasFiredWhileHolding = false;
        _timeSinceLastFired = 999f;
        _stackAcc = 0f;
        _stacks = 0;
        _appliedStacks = 0;

        if (_statsContext != null)
            _statsContext.SetDynamicMods(_dynOwnerId, null);
    }

    private void Clear()
    {
        _isHold = false;

        ClearDynamic();

        _spinTime = 0f;
        _spunUp = false;

        _cachedMode = RiflePerkMode.None;
    }
}
