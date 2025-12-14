using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseData", menuName = "ScriptableObject/Phase")]
public class PhaseData : ScriptableObject
{
    // 얘는 프리팹 정보만 가진 리스트 (인스펙터에서 할당)
    [SerializeField] private List<PatternBase> _patterns;
    public List<PatternBase> Pattern => _patterns;
}
