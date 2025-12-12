using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseData", menuName = "ScriptableObject/Phase")]
public class PhaseData : ScriptableObject
{
    [SerializeField] private List<PatternBase> patterns;
}
