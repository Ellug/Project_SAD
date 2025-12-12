using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _cycleTime;
    WaitForSeconds patternDelay;

    void Awake()
    {
        patternDelay = new WaitForSeconds(_cycleTime);
    }

    public void StartPatternTimer()
    {
        StartCoroutine(PatternCycle());
    }

    public void StopPatternTimer()
    {
        StopCoroutine(PatternCycle());
    }

    protected virtual IEnumerator PatternCycle()
    {
        PatternLogic();
        yield return patternDelay;
    }

    protected abstract void PatternLogic();
}
