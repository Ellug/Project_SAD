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

    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    private PerksTree _tree;
    private int _stageIndex;

    public event Action<int, int> OnOptionClicked; // (stageIndex, side)

    public void ApplyStageNodes(PerksTree tree, int stageIndex)
    {
        _tree = tree;
        _stageIndex = stageIndex;

        if (_stageText != null)
            _stageText.text = $"Level {stageIndex + 1}";

        if (_leftButton != null)
        {
            _leftButton.onClick.RemoveAllListeners();
            _leftButton.onClick.AddListener(() =>
            {
                _tree?.Select(_stageIndex, PerksTree.SideLeft);
                OnOptionClicked?.Invoke(_stageIndex, PerksTree.SideLeft);
                Refresh();
            });
        }

        if (_rightButton != null)
        {
            _rightButton.onClick.RemoveAllListeners();
            _rightButton.onClick.AddListener(() =>
            {
                _tree?.Select(_stageIndex, PerksTree.SideRight);
                OnOptionClicked?.Invoke(_stageIndex, PerksTree.SideRight);
                Refresh();
            });
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_tree == null) return;

        int selected = _tree.GetSelectedSide(_stageIndex);

        SetSprite(_leftButton, selected == PerksTree.SideLeft);
        SetSprite(_rightButton, selected == PerksTree.SideRight);
    }

    private void SetSprite(Button button, bool isSelected)
    {
        if (button == null) return;

        var img = button.targetGraphic as Image;
        if (img == null) return;

        img.sprite = isSelected ? _selectedSprite : _normalSprite;
    }
}
