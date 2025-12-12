using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private BossController boss;
    [SerializeField] private List<PhaseData> phase;

    void Awake()
    {
        phase = new List<PhaseData>();
        boss._phaseChange += ChangePhase;    
    }

    void OnDestroy()
    {
        boss._phaseChange -= ChangePhase;
    }

    public void ChangePhase()
    {
        // 페이즈 변환 시 일어날 일
    }
}
