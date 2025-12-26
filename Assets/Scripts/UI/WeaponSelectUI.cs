using System;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUI : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public int weaponId;
        public Button button;
        public Image targetImage;
    }

    [SerializeField] private Entry[] _entries;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _selectedSprite;

    private int _selectedWeaponId = -1;

    // 외부에서 구독해서 실제 무기 선택 로직 실행
    public event Action<int> OnWeaponClicked;

    void Awake()
    {
        // 버튼 클릭 연결
        for (int i = 0; i < _entries.Length; i++)
        {
            int id = _entries[i].weaponId;
            if (_entries[i].button == null) continue;

            _entries[i].button.onClick.RemoveAllListeners();
            _entries[i].button.onClick.AddListener(() => Select(id));
        }
    }

    public void Select(int weaponId)
    {
        _selectedWeaponId = weaponId;
        RefreshVisual();

        OnWeaponClicked?.Invoke(weaponId);
    }

    public void SetSelectedVisualOnly(int weaponId)
    {
        _selectedWeaponId = weaponId;
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        for (int i = 0; i < _entries.Length; i++)
        {
            var e = _entries[i];
            if (e.targetImage == null) continue;

            bool isSelected = e.weaponId == _selectedWeaponId;
            e.targetImage.sprite = isSelected ? _selectedSprite : _normalSprite;
        }
    }
}
