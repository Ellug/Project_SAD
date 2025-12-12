using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private BossController _boss;
    [SerializeField] private List<PhaseData> _phase;
    private PhaseData _curPhase;
    private int _phaseIndex;

    void Awake()
    {
        _boss._phaseChange += ChangePhase;
        _phaseIndex = 0;
        _curPhase = _phase[_phaseIndex];
    }

    void Start()
    {
        _curPhase.StartPhase();
    }

    void OnDestroy()
    {
        _boss._phaseChange -= ChangePhase;
    }

    public void ChangePhase()
    {
        if (_phaseIndex < _phase.Count - 1)
        {
            // 지금 페이즈 멈추고, 다음 페이즈로 전환 후 시작해라.
            _curPhase.StopPhase();
            _curPhase = _phase[++_phaseIndex];
            _curPhase.StartPhase();
        }
    }
}
