using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _cycleTime;
    private WaitForSeconds _patternDelay;
    private Coroutine _patternCoroutine;

    void Awake()
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

    protected abstract void PatternLogic();
}
