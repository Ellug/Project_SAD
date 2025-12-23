using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerksItemUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text _stageText;

    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [SerializeField] private TMP_Text _leftButtonText;
    [SerializeField] private TMP_Text _rightButtonText;

    private PerksTree _tree;
    private int _stageIndex;

    public void ApplyStageNodes(PerksTree tree, int stageIndex)
    {
        _tree = tree;
        _stageIndex = stageIndex;

        if (_stageText != null)
            _stageText.text = $"Stage {stageIndex + 1}";

        var leftNode = _tree.GetNode(stageIndex, PerksTree.SideLeft);
        var rightNode = _tree.GetNode(stageIndex, PerksTree.SideRight);

        if (_leftButtonText != null)
            _leftButtonText.text = leftNode == null ? "-" : leftNode.Description;

        if (_rightButtonText != null)
            _rightButtonText.text = rightNode == null ? "-" : rightNode.Description;

        if (_leftButton != null)
        {
            _leftButton.onClick.RemoveAllListeners();
            _leftButton.onClick.AddListener(() => _tree.Select(_stageIndex, PerksTree.SideLeft));
        }

        if (_rightButton != null)
        {
            _rightButton.onClick.RemoveAllListeners();
            _rightButton.onClick.AddListener(() => _tree.Select(_stageIndex, PerksTree.SideRight));
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_tree == null) return;

        int selected = _tree.GetSelectedSide(_stageIndex);

        ApplyButtonStyle(_leftButton, _leftButtonText, isSelected: selected == PerksTree.SideLeft);
        ApplyButtonStyle(_rightButton, _rightButtonText, isSelected: selected == PerksTree.SideRight);
    }

    private void ApplyButtonStyle(Button button, TMP_Text text, bool isSelected)
    {
        if (button != null)
        {
            Image img = button.GetComponent<Image>();
            if (img != null) img.color = isSelected ? Color.red : Color.white;
        }

        if (text != null) text.color = isSelected ? Color.white : Color.black;
    }
}
