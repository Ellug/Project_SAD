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
    private Coroutine _patternCoroutine;
    private bool _isReadyCounterAttack;
    private bool _isTakeCounterAttack;

    protected virtual void Awake()
    {
        _patternDelay = new WaitForSeconds(_cycleTime);
        _isReadyCounterAttack = false;
        _isTakeCounterAttack = false;
    }

    public void StartPatternTimer()
    {
        _patternCoroutine = StartCoroutine(PatternCycle());
    }

    public void StopPatternTimer()
    {
        StopCoroutine(_patternCoroutine);
    }

    public void CounterAttackTrigger()
    {
        if (_counterable && _isReadyCounterAttack)
        {
            _isTakeCounterAttack = true;
        }
    }

    protected IEnumerator PatternCycle()
    {
        while (true)
        {
            yield return _patternDelay;
            if (_counterable)
            {
                _isReadyCounterAttack = true;
                // TODO : 카운터 준비 상태일 때 보스의 색상이 바뀌어 시각적 피드백 제공
                Debug.Log("강력한 공격 준비 상태 진입!");
                // 카운터 가능한 공격은 준비 동작을 먼저 실행
                yield return StartCoroutine(CounterableAttackReady());
                _isReadyCounterAttack = false;
                if (_isTakeCounterAttack == false)
                {
                    Debug.Log("강력한 공격 발동!");
                    PatternLogic();
                }
                else
                {
                    Debug.Log("강력한 공격 취소됨!");
                    // 패턴 실행하지 않고 bool 값 원복
                    _isTakeCounterAttack = false;
                } 
            }
            else
            {
                PatternLogic();
            }
        }
    }

    private IEnumerator CounterableAttackReady()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _startupDelay) 
        {
            // 매 프레임 bool 값을 검사하다가 이것이 true가 되는 순간 탈출함.
            // 이 bool 값은 어딘가에서 값을 바꿔줘야 함.
            yield return null;
            elapsedTime += Time.deltaTime;
            if (_isTakeCounterAttack)
            {
                yield break;
            }
        }
        // 만약 준비시간 동안 bool 값이 변하지 않았다면 카운터 실패로 패턴을 수행함.
    }

    public abstract void Init(GameObject target);
    protected abstract void PatternLogic();
}
