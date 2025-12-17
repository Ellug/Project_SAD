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

    private void EnsureBuilt()
    {
        int stageCount = StageCount;
        if (stageCount <= 0) return;

        if (_nodes != null && _selected != null && _selected.Length == stageCount)
            return;

        // 인스펙터 데이터 -> 런타임 배열로 빌드
        _nodes = new PerksNode[stageCount, SideCount];
        _selected = new int[stageCount];

        for (int s = 0; s < stageCount; s++)
        {
            _nodes[s, SideLeft] = _stageDefs[s].left;
            _nodes[s, SideRight] = _stageDefs[s].right;
            _selected[s] = -1;
        }
    }

    public PerksNode GetNode(int stageIndex, int sideIndex)
    {
        EnsureBuilt();

        if (!IsValid(stageIndex, sideIndex)) return null;

        return _nodes[stageIndex, sideIndex];
    }

    public int GetSelectedSide(int stageIndex)
    {
        EnsureBuilt();

        if (!IsValidStage(stageIndex)) return -1;

        return _selected[stageIndex];
    }

    public PerksNode GetSelectedNode(int stageIndex)
    {
        EnsureBuilt();

        if (!IsValidStage(stageIndex)) return null;

        int side = _selected[stageIndex];

        return (side == -1) ? null : _nodes[stageIndex, side];
    }

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

    public IEnumerable<StatMod> GetActiveMods()
    {
        EnsureBuilt();

        if (_nodes == null || _selected == null) yield break;

        for (int s = 0; s < _selected.Length; s++)
        {
            var node = GetSelectedNode(s);
            if (node?.mods == null) continue;

            for (int i = 0; i < node.mods.Length; i++)
                yield return node.mods[i];
        }
    }

    public int[] ExportSelections()
    {
        EnsureBuilt();

        if (_selected == null)
            return Array.Empty<int>();

        return (int[])_selected.Clone();
    }

    public void ImportSelections(int[] selections, bool notify = true)
    {
        EnsureBuilt();

        if (_selected == null || selections == null) return;

        int count = Mathf.Min(_selected.Length, selections.Length);
        for (int i = 0; i < count; i++)
        {
            int v = selections[i];
            _selected[i] = (v == -1 || v == 0 || v == 1) ? v : -1;
        }

        if (notify) OnChanged?.Invoke();
    }

    private bool IsValidStage(int stageIndex) => _selected != null && stageIndex >= 0 && stageIndex < _selected.Length;
    private bool IsValid(int stageIndex, int sideIndex) => IsValidStage(stageIndex) && (sideIndex == 0 || sideIndex == 1);
}
