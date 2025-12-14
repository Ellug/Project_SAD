using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private BossController _boss;
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private List<PhaseData> _phase;

    private PhaseData _curPhase;
    private int _phaseIndex;

    // 런타임 중 프리팹을 인스턴스한 오브젝트를 가진 배열
    private PatternBase[] _activeObjects;

    void Awake()
    {
        _boss._phaseChange += ChangePhase;
        _boss._takeCounterableAttack += TriggerCounter;
        _phaseIndex = 0;
        _curPhase = _phase[_phaseIndex];
    }

    void Start()
    {
        StartCurrentPhase();
    }

    void OnDestroy()
    {
        _boss._phaseChange -= ChangePhase;
        _boss._takeCounterableAttack -= TriggerCounter;
    }

    private void StartCurrentPhase()
    {
        _activeObjects = new PatternBase[_curPhase.Pattern.Count];
        // 담겨진 패턴들 모두 타이머 시작
        for (int i = 0; i < _activeObjects.Length; i++)
        {
            _activeObjects[i] = Instantiate(_curPhase.Pattern[i].gameObject).GetComponent<PatternBase>();
            _activeObjects[i].Init(_player);
            _activeObjects[i].StartPatternTimer();
        }
    }

    private void StopCurrentPhase()
    {
        for (int i = 0; i < _activeObjects.Length; i++)
        {
            _activeObjects[i].StopPatternTimer();
            _activeObjects[i].gameObject.SetActive(false);
        }
    }

    public void ChangePhase()
    {
        if (_phaseIndex < _phase.Count - 1)
        {
            // 지금 페이즈 멈추고, 다음 페이즈로 전환 후 시작해라.
            StopCurrentPhase();
            _curPhase = _phase[++_phaseIndex];
            StartCurrentPhase();
        }
    }

    public void TriggerCounter()
    {
        foreach (PatternBase pattern in _activeObjects) 
        {
            pattern.CounterAttackTrigger();
        }
    }
}
