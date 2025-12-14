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

    protected void Awake()
    {
        _patternDelay = new WaitForSeconds(_cycleTime);
    }

    public void StartPatternTimer()
    {
        _patternCoroutine = StartCoroutine(PatternCycle());
    }

    public void StopPatternTimer()
    {
        StopCoroutine(_patternCoroutine);
    }

    protected IEnumerator PatternCycle()
    {
        while (true)
        {
            yield return _patternDelay;
            PatternLogic();
        }
    }
    public abstract void Init(GameObject target);
    protected abstract void PatternLogic();
}
