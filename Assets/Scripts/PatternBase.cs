using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _cycleTime;
    private WaitForSeconds patternDelay;
    private Coroutine patternCoroutine;

    void Awake()
    {
        patternDelay = new WaitForSeconds(_cycleTime);
    }

    public void StartPatternTimer()
    {
        patternCoroutine = StartCoroutine(PatternCycle());
    }

    public void StopPatternTimer()
    {
        StopCoroutine(patternCoroutine);
    }

    protected IEnumerator PatternCycle()
    {
        while (true)
        {
            yield return patternDelay;
            PatternLogic();
        }
    }

    protected abstract void PatternLogic();
}
