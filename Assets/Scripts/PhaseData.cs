using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseData", menuName = "ScriptableObject/Phase")]
public class PhaseData : ScriptableObject
{
    // 얘는 프리팹 정보만 가진 리스트 (인스펙터에서 할당)
    [SerializeField] private List<PatternBase> patterns;
    
    // 얘는 런타임 중 프리팹을 인스턴스한 오브젝트를 가진 리스트
    private PatternBase[] activeObjects;

    public List<PatternBase> Pattern { get { return patterns; } }

    public void StartPhase()
    {
        activeObjects = new PatternBase[patterns.Count];
        // 담겨진 패턴들 모두 타이머 시작
        for (int i = 0; i < activeObjects.Length; i++)
        {
            activeObjects[i] = Instantiate(Pattern[i].gameObject).GetComponent<PatternBase>();
            activeObjects[i].StartPatternTimer();
        }
    }

    public void StopPhase()
    {
        for (int i = 0; i < activeObjects.Length; i++)
        {
            activeObjects[i].StopPatternTimer();
            activeObjects[i].gameObject.SetActive(false);
        }
    }
}
