using System;
using System.Collections.Generic;
using UnityEngine;

public class PerksTree : MonoBehaviour
{
    public const int SideLeft = 0;
    public const int SideRight = 1;
    private const int SideCount = 2;

    [Serializable]
    public class StageDef
    {
        public PerksNode left;
        public PerksNode right;
    }

    [Header("Perks Setting")]
    [SerializeField] private List<StageDef> _stageDefs = new();

    private PerksNode[,] _nodes;   // [stage, side]
    private int[] _selected;       // [-1,0,1] per stage

    public event Action OnChanged;

    public int StageCount => _stageDefs?.Count ?? 0;

    // 인스펙터 데이터(StageDef) -> 배열 구조로 변환
    private void EnsureBuilt()
    {
        int stageCount = StageCount;
        if (stageCount <= 0) return;

        // 이미 빌드되어 있고, 스테이지 수가 일치하면 재빌드x
        if (_nodes != null && _selected != null && _selected.Length == stageCount)
            return;

        // 인스펙터 데이터 -> 런타임 배열로 빌드
        _nodes = new PerksNode[stageCount, SideCount]; // 스테지이 수 x 2 노드 배열 생성
        _selected = new int[stageCount];

        // 각 스테이지 노드 배열로 처리
        for (int s = 0; s < stageCount; s++)
        {
            _nodes[s, SideLeft] = _stageDefs[s].left;
            _nodes[s, SideRight] = _stageDefs[s].right;
            _selected[s] = -1;
        }
    }

    // 스테이지 배열에서 좌/우 노드드 조회 -> 유효성 검사 -> 해당 노드 반환
    public PerksNode GetNode(int stageIndex, int sideIndex)
    {
        EnsureBuilt();

        if (!IsValid(stageIndex, sideIndex)) return null;

        return _nodes[stageIndex, sideIndex];
    }

    // 특정 스테이지에서 선택된 side 반환 -> int로 List 관리를 위함
    public int GetSelectedSide(int stageIndex)
    {
        EnsureBuilt();

        if (!IsValidStage(stageIndex)) return -1;

        return _selected[stageIndex];
    }

    // 현재 스테이지에서 현재 선택된 PerksNode 반환
    public PerksNode GetSelectedNode(int stageIndex)
    {
        EnsureBuilt();

        if (!IsValidStage(stageIndex)) return null;

        int side = _selected[stageIndex];

        return (side == -1) ? null : _nodes[stageIndex, side];
    }

    // 스테이지의 노드 선택 토글
    public void Select(int stageIndex, int sideIndex)
    {
        EnsureBuilt();

        if (!IsValid(stageIndex, sideIndex)) return;

        if (_selected[stageIndex] == sideIndex)
            _selected[stageIndex] = -1;
        else
            _selected[stageIndex] = sideIndex;

        OnChanged?.Invoke();
    }

    // 현재 선택된 모든 특전의 StatMod를 순회 반환 -> 실제 능력치 계산
    // 각 스테이지에서 선택된 노드의 mods를 합쳐서 제공
    // 실제 능력치 반영은 PerksCalculator가 받아서 처리
    public IEnumerable<StatMod> GetActiveMods()
    {
        EnsureBuilt();

        if (_nodes == null || _selected == null) yield break;

        for (int s = 0; s < _selected.Length; s++) // 스테이지 순회
        {
            var node = GetSelectedNode(s); // 현재 스테이지의 선택 노드 Get
            if (node?.mods == null) continue; // 없으면 스킵

            for (int i = 0; i < node.mods.Length; i++) // 노드의 효과 목록 순회
                yield return node.mods[i];  // 하나씩 내보냄
        }
    }

    // 선택된 노드의 버프들을 전부 반환
    public IEnumerable<TriggeredBuff> GetActiveBuffs()
    {
        EnsureBuilt();

        if (_nodes == null || _selected == null) yield break;

        for (int s = 0; s < _selected.Length; s++)
        {
            var node = GetSelectedNode(s);
            if (node?.buffs == null) continue;

            for (int i = 0; i < node.buffs.Length; i++)
                yield return node.buffs[i];
        }
    }

    // 현재 선택 상태를 외부 저장용 배열로 반환 -> GM
    public int[] ExportSelections()
    {
        EnsureBuilt();

        if (_selected == null)
            return Array.Empty<int>();

        return (int[])_selected.Clone(); // 캡슐화를 위한 클론 반환
    }

    // 외부에서 저장된 선택 상태 복구
    public void ImportSelections(int[] selections, bool notify = true)
    {
        EnsureBuilt();

        if (_selected == null || selections == null) return;

        int count = Mathf.Min(_selected.Length, selections.Length);
        for (int i = 0; i < count; i++)
        {
            int v = selections[i];
            _selected[i] = (v == -1 || v == 0 || v == 1) ? v : -1; // 유효값 아니면 -1
        }

        if (notify) OnChanged?.Invoke();
    }

    // 유효성 검사 람다
    private bool IsValidStage(int stageIndex) => _selected != null && stageIndex >= 0 && stageIndex < _selected.Length;
    private bool IsValid(int stageIndex, int sideIndex) => IsValidStage(stageIndex) && (sideIndex == 0 || sideIndex == 1);
}
