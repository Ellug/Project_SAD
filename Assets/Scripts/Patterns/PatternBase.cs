using System;
using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    public enum OptionMode { 원형, 직선, None }

    [Header("경고장판 속성")]
    [SerializeField, Tooltip("경고장판 형태")] protected OptionMode mode;
    [SerializeField, Tooltip("경고장판 파티클")] protected ParticleSystem _WarnningArea;
    [SerializeField, Tooltip("경고장판 추적 시간")] protected float _WarnningTime;
    [SerializeField, Tooltip("경고장판 정지 후 대기시간")] protected float _WarnningDTime;
    [SerializeField, Tooltip("장판 최대 길이(직선용)")] protected float _WarnningMaxLength;
    [SerializeField, Tooltip("장판 너비(직선용)")] protected float _WarnningWidth;
    [SerializeField, Tooltip("장판 길이 보정 배율")] protected float _lengthScaleModifier;
    [SerializeField, Tooltip("레이어 마스크")] protected LayerMask _groundLayer;
    [SerializeField, Tooltip("플레이어 예측치(0이면 플레이어 위치)")] protected float _ChaseOffset;

    [Header("고정 스폰 설정")]
    [SerializeField, Tooltip("체크 시 스폰 포인트의 위치와 방향으로 고정")] protected bool _useFixedSpawnPoint;

    [Header("카운터 관련 속성")]
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _startupDelay;

    [Header("공용 패턴 속성")]
    [SerializeField, Tooltip("첫 패턴 시작 전 대기 시간")] protected float _startDelay = 1f;
    [SerializeField] protected float _cycleTime;

    protected ParticleSystem _currentWarning;
    protected Transform _warningTransform;
    protected GameObject _target;
    protected bool _isPatternActive;
    protected BossController _boss;
    protected Vector3 _lastDirection;
    protected PredictiveAim _predictiveAim;

    public event Action<PatternEnum> OnPatternSound;

    protected virtual void Awake()
    {
        _isReadyCounter = false;
        _isCounterTaken = false;
    }

    private void Start()
    {
        _boss = GameObject.FindAnyObjectByType<BossController>();
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected virtual void Update()
    {
        if (_isPatternActive && _warningTransform != null && _target != null)
        {
            UpdateWarningPosition();
        }
    }

    private void UpdateWarningPosition()
    {
        Vector3 origin = transform.position;
        origin.y = 0.1f;

        Vector3 targetPos;
        Vector3 direction;

        if (_useFixedSpawnPoint)
        {
            targetPos = transform.position;
            direction = transform.forward;
        }
        else
        {
            targetPos = _predictiveAim != null ? _predictiveAim.PredictiveAimCalc(_ChaseOffset) : _target.transform.position;
            targetPos.y = 0.1f;
            direction = (targetPos - origin).normalized;
        }

        if (direction == Vector3.zero) return;

        _lastDirection = direction;

        if (mode == OptionMode.직선)
        {
            float distance = _WarnningMaxLength;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, _WarnningMaxLength, _groundLayer))
                distance = hit.distance;

            _warningTransform.rotation = Quaternion.LookRotation(direction);
            _warningTransform.localScale = new Vector3(_WarnningWidth, 1f, distance * _lengthScaleModifier);
            _warningTransform.position = origin + direction * (distance * 0.5f);
        }
        else if (mode == OptionMode.원형)
        {
            _warningTransform.position = targetPos;
            _warningTransform.rotation = Quaternion.identity;
        }
    }

    protected IEnumerator ShowWarning()
    {
        if (_WarnningArea != null)
        {
            Vector3 startPos;
            if (_useFixedSpawnPoint)
            {
                startPos = transform.position;
            }
            else
            {
                startPos = _predictiveAim != null ? _predictiveAim.PredictiveAimCalc(_ChaseOffset) : _target.transform.position;
            }

            _currentWarning = PoolManager.Instance.Spawn(_WarnningArea, startPos, Quaternion.identity);
            _warningTransform = _currentWarning.transform;
            _isPatternActive = true;
            _currentWarning.Play();
        }

        yield return new WaitForSeconds(_WarnningTime);

        _isPatternActive = false;
        yield return new WaitForSeconds(_WarnningDTime);
    }

    protected void RemoveWarning()
    {
        if (_currentWarning != null)
        {
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }
    }

    #region Pattern Cycle Control
    public void StartPatternCycle()
    {
        if (_patternCycleCoroutine == null)
            _patternCycleCoroutine = StartCoroutine(PatternCycle());
    }

    public void StopPatternCycle()
    {
        if (_patternCycleCoroutine != null)
        {
            StopCoroutine(_patternCycleCoroutine);
            _patternCycleCoroutine = null;
        }
        ForceStopCurrentPattern();
    }

    private Coroutine _patternCycleCoroutine;
    private Coroutine _currentPatternCoroutine;
    private bool _isReadyCounter;
    private bool _isCounterTaken;

    private IEnumerator PatternCycle()
    {
        if (_startDelay > 0)
        {
            yield return new WaitForSeconds(_startDelay);
        }

        WaitForSeconds delay = new WaitForSeconds(_cycleTime);
        while (true)
        {
            if (_counterable)
            {
                _isReadyCounter = true;
                _boss?.UpdateDebuffVisual(_boss._CounterMaterial, true);
                yield return StartCoroutine(CounterableDelay());
                _isReadyCounter = false;

                if (!_isCounterTaken) ExecutePattern();
                else _isCounterTaken = false;

                _boss?.UpdateDebuffVisual(_boss._CounterMaterial, false);
            }
            else
            {
                ExecutePattern();
            }

            yield return delay;
        }
    }

    private IEnumerator CounterableDelay()
    {
        float elapsed = 0f;
        while (elapsed < _startupDelay)
        {
            if (_isCounterTaken) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void TriggerCounter()
    {
        if (_counterable && _isReadyCounter)
            _isCounterTaken = true;
    }
    #endregion

    #region Pattern Execution
    protected void ExecutePattern()
    {
        ForceStopCurrentPattern();
        _currentPatternCoroutine = StartCoroutine(PatternRoutine());
    }

    protected abstract IEnumerator PatternRoutine();

    protected void ForceStopCurrentPattern()
    {
        _isPatternActive = false;
        if (_currentPatternCoroutine != null)
        {
            StopCoroutine(_currentPatternCoroutine);
            _currentPatternCoroutine = null;
        }
        RemoveWarning();
        CleanupPattern();
    }

    protected abstract void CleanupPattern();
    #endregion

    protected void PlayPatternSound(PatternEnum patternEnum) => OnPatternSound?.Invoke(patternEnum);
    public virtual void Init(GameObject target)
    {
        _target = target;
    }
}