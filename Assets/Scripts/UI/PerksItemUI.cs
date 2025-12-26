using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerksItemUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text _stageText;

    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    private PerksTree _tree;
    private int _stageIndex;

    // Panel이 구독해서 어느 스테이지/어느 쪽이 클릭됐는지 알 수 있게 함
    public event Action<int, int> OnOptionClicked; // (stageIndex, side)

    public void ApplyStageNodes(PerksTree tree, int stageIndex)
    {
        _tree = tree;
        _stageIndex = stageIndex;

        if (_stageText != null)
            _stageText.text = $"Stage {stageIndex + 1}";

        if (_leftButton != null)
        {
            _leftButton.onClick.RemoveAllListeners();
            _leftButton.onClick.AddListener(() =>
            {
                if (_tree != null)
                    _tree.Select(_stageIndex, PerksTree.SideLeft);

                OnOptionClicked?.Invoke(_stageIndex, PerksTree.SideLeft);
            });
        }

        if (_rightButton != null)
        {
            _rightButton.onClick.RemoveAllListeners();
            _rightButton.onClick.AddListener(() =>
            {
                if (_tree != null)
                    _tree.Select(_stageIndex, PerksTree.SideRight);

                OnOptionClicked?.Invoke(_stageIndex, PerksTree.SideRight);
            });
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_tree == null) return;

        int selected = _tree.GetSelectedSide(_stageIndex);

        ApplyButtonStyle(_leftButton, isSelected: selected == PerksTree.SideLeft);
        ApplyButtonStyle(_rightButton, isSelected: selected == PerksTree.SideRight);
    }

    private void ApplyButtonStyle(Button button, bool isSelected)
    {
        if (button == null) return;

        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color = isSelected ? Color.red : Color.white;
    }
}
