using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseData", menuName = "ScriptableObject/Phase")]
public class PhaseData : ScriptableObject
{
    [SerializeField] private List<PatternBase> patterns;

    public List<PatternBase> Pattern { get { return patterns; } }

    public void StartPhase()
    {
        for (int i = 0; i < Pattern.Count; i++)
        {
            Pattern[i].StartPatternTimer();
        }
    }

    public void StopPhase()
    {
        for (int i = 0; i < Pattern.Count; i++)
        {
            Pattern[i].StopPatternTimer();
        }
    }
}
