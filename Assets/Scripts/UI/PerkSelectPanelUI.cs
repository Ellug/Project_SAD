using System.Collections.Generic;
using UnityEngine;

public class PerkSelectPanelUI : MonoBehaviour
{
    [Header("Prefab / Root")]
    [SerializeField] private PerksItemUI _stagePrefab;
    [SerializeField] private Transform _root;

    private List<PerksItemUI> _items = new();
    private PerksTree _tree;

    void Awake()
    {
        if (_root == null)
            _root = transform;
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnWeaponEquipped += HandleWeaponEquipped;
    }

    void Start()
    {
        // 로비 진입 직후 이미 장착된 경우 처리
        if (GameManager.Instance != null && GameManager.Instance.CurrentWeaponInstance != null)
            HandleWeaponEquipped(GameManager.Instance.CurrentWeaponInstance);
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnWeaponEquipped -= HandleWeaponEquipped;
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
            PerksItemUI item = Instantiate(_stagePrefab, _root);
            item.gameObject.SetActive(true);
            item.ApplyStageNodes(_tree, i);
            _items.Add(item);
        }

        _tree.OnChanged += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].Refresh();
    }

    public void Clear()
    {
        if (_tree != null)
            _tree.OnChanged -= Refresh;

        _tree = null;

        for (int i = 0; i < _items.Count; i++)
            if (_items[i] != null) Destroy(_items[i].gameObject);
        _items.Clear();
    }

    void OnDestroy()
    {
        Clear();
    }
}
