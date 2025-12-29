using System;
using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    [Header("카운터 관련 속성")]
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _startupDelay;

    [Header("공용 패턴 속성")]
    [SerializeField] protected float _cycleTime;

    private WaitForSeconds _patternDelay;
    private Coroutine _patternCycleCoroutine;
    private Coroutine _currentPatternCoroutine;

    private bool _isReadyCounter;
    private bool _isCounterTaken;

    protected bool _isPatternActive; // Update 제어용
    protected BossController _boss;

    public event Action<PatternEnum> OnPatternSound;

    protected virtual void Awake()
    {
        _patternDelay = new WaitForSeconds(_cycleTime);
        _isReadyCounter = false;
        _isCounterTaken = false;
    }

    private void Start()
    {
        _boss = GameObject.FindAnyObjectByType<BossController>();
    }
    protected virtual void Update()
    { 
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

    private IEnumerator PatternCycle()
    {
        while (true)
        {
            yield return _patternDelay;

            if (_counterable)
            {
                _isReadyCounter = true;
                _boss?.UpdateDebuffVisual(_boss._CounterMaterial, true);
                yield return StartCoroutine(CounterableDelay());
                _isReadyCounter = false;

                if (!_isCounterTaken)
                {
                    ExecutePattern();
                }
                else
                {
                    _isCounterTaken = false;
                }

                _boss?.UpdateDebuffVisual(_boss._CounterMaterial, false);
            }
            else
            {
                ExecutePattern();
            }
        }
    }

    private IEnumerator CounterableDelay()
    {
        float elapsed = 0f;
        while (elapsed < _startupDelay)
        {
            if (_isCounterTaken)
                yield break;

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
        ForceStopCurrentPattern(); // 이전 패턴 종료
        _isPatternActive = true;
        _currentPatternCoroutine = StartCoroutine(PatternRoutine());
    }

    protected virtual IEnumerator PatternRoutine()
    {
        // 실제 패턴 로직은 여기서 구현
        yield break;
    }

    protected void ForceStopCurrentPattern()
    {
        _isPatternActive = false;

        if (_currentPatternCoroutine != null)
        {
            StopCoroutine(_currentPatternCoroutine);
            _currentPatternCoroutine = null;
        }

        CleanupPattern();
    }

    protected abstract void CleanupPattern(); // 파티클 제거, 상태 초기화 등
    #endregion

    protected void PlayPatternSound(PatternEnum patternEnum)
    {
        OnPatternSound?.Invoke(patternEnum);
    }

    public abstract void Init(GameObject target);
}
