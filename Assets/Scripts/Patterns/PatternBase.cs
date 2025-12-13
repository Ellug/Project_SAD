using System.Collections;
using UnityEngine;

public abstract class PatternBase : MonoBehaviour
{
    [SerializeField] protected bool _counterable;
    [SerializeField] protected float _cycleTime;
    private WaitForSeconds _patternDelay;
    private Coroutine _patternCoroutine;

    protected void Awake()
    {
        Debug.Log("패턴 딜레이 클래스 생성.");
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
            Debug.Log($"패턴 로직을 가동. ({_cycleTime}초마다 호출해야 함)");
            PatternLogic();
        }
    }
    public abstract void Init(GameObject target);
    protected abstract void PatternLogic();
}
