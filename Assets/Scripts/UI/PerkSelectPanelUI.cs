using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerkSelectPanelUI : MonoBehaviour
{
    [Header("Prefab / Root")]
    [SerializeField] private PerksItemUI _stagePrefab;

    [Header("UI")]
    [SerializeField] private TMP_Text _desc;

    private readonly List<PerksItemUI> _items = new();
    private PerksTree _tree;

    private int _lastStage = -1;
    private int _lastSide = -1;

    void OnEnable()
    {
        if (EquipManager.Instance != null)
            EquipManager.Instance.OnWeaponEquipped += HandleWeaponEquipped;
    }

    void Start()
    {
        if (EquipManager.Instance != null && EquipManager.Instance.CurrentWeaponInstance != null)
            HandleWeaponEquipped(EquipManager.Instance.CurrentWeaponInstance);
    }

    void OnDisable()
    {
        if (EquipManager.Instance != null)
            EquipManager.Instance.OnWeaponEquipped -= HandleWeaponEquipped;
    }

    private void HandleWeaponEquipped(WeaponBase weapon)
    {
        ApplyPerksTree(weapon != null ? weapon.PerksTree : null);
    }

    public void ApplyPerksTree(PerksTree tree)
    {
        Clear();

        _tree = tree;
        if (_tree == null || _stagePrefab == null) return;

        int count = _tree.StageCount;
        for (int i = 0; i < count; i++)
        {
            PerksItemUI item = Instantiate(_stagePrefab, transform);
            item.gameObject.SetActive(true);

            item.ApplyStageNodes(_tree, i);

            // 클릭 콜백 연결
            item.OnOptionClicked += HandleOptionClicked;

            _items.Add(item);
        }

        _tree.OnChanged += Refresh;

        // 초기 표시
        _lastStage = 0;
        _lastSide = -1;
        Refresh();
    }

    private void HandleOptionClicked(int stageIndex, int side)
    {
        _lastStage = stageIndex;
        _lastSide = side;

        // 선택 직후(또는 OnChanged 직후) 설명 갱신
        RefreshSelectionTexts();
    }

    private void Refresh()
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].Refresh();

        RefreshSelectionTexts();
    }

    private void RefreshSelectionTexts()
    {
        if (_tree == null)
        {
            SetTexts("");
            return;
        }

        int stageCount = _tree.StageCount;

        // 진행도(선택된 스테이지 수) 계산
        int selectedCount = 0;
        for (int i = 0; i < stageCount; i++)
            if (_tree.GetSelectedSide(i) >= 0)
                selectedCount++;

        // 표시 기준 스테이지 결정
        int stage = (_lastStage >= 0 && _lastStage < stageCount) ? _lastStage : 0;

        int selectedSide = _tree.GetSelectedSide(stage);

        if (selectedSide < 0)
        {
            SetTexts("-");
            return;
        }

        var node = _tree.GetNode(stage, selectedSide);
        string desc = node != null ? node.Description : "";

        SetTexts(desc);
    }

    private void SetTexts(string desc)
    {
        if (_desc != null) _desc.text = desc;
    }

    public void Clear()
    {
        if (_tree != null)
            _tree.OnChanged -= Refresh;

        // 아이템 이벤트 해제 + 제거
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i] == null) continue;

            _items[i].OnOptionClicked -= HandleOptionClicked;
            Destroy(_items[i].gameObject);
        }
        _items.Clear();

        _tree = null;
        _lastStage = -1;
        _lastSide = -1;

        SetTexts("");
    }

    void OnDestroy()
    {
        Clear();
    }
}
