using TMPro;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private GameObject _weaponSelectMainUI;
    [SerializeField] private PerkSelectPanelUI _perkPanelUI;
    [SerializeField] private WeaponBase[] _allWeaponData;

    [Header("UI Selection (visual only)")]
    [SerializeField] private WeaponSelectUI _weaponSelectUI;

    private WeaponPresenter _presenter;
    private TextMeshProUGUI _titleText;

    void Start()
    {
        Init();

        // 기본 무기 표시(EquipManager 기준) - WeaponSelectUI가 표시 담당
        int defaultId = 0;
        if (EquipManager.Instance != null && EquipManager.Instance.Weapon != null)
            defaultId = EquipManager.Instance.Weapon.GetWeaponId();

        if (_weaponSelectUI != null)
            _weaponSelectUI.SetSelectedVisualOnly(defaultId);

        WeaponModel model = new();
        model.Init(_allWeaponData);
        _presenter = new WeaponPresenter(model, this);
        _presenter.SelectWeapon(defaultId);
        _presenter.Init();
    }

    void OnEnable()
    {
        if (_weaponSelectUI != null)
        {
            _weaponSelectUI.OnWeaponClicked -= OnClickWeapon;
            _weaponSelectUI.OnWeaponClicked += OnClickWeapon;
        }
    }

    void OnDisable()
    {
        if (_weaponSelectUI != null)
            _weaponSelectUI.OnWeaponClicked -= OnClickWeapon;
    }

    void OnDestroy()
    {
        if (_weaponSelectUI != null)
            _weaponSelectUI.OnWeaponClicked -= OnClickWeapon;
    }

    // 무기 버튼과 특전 버튼을 모두 가져옴
    private void Init()
    {
        _titleText = _weaponSelectMainUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    // 무기를 선택하면 일어날 일.
    // 무기 정보도 가져오고 무기별 특전도 가져와야함.
    // UI 담당자 : 이제 무기 UI가 로비에 기본으로 있고, 무기 UI 누르면 바로 특전 UI 띄움.
    // UI 담당자 : 추가로 UI 떠있는 상태에서 다른 무기 누르면 정보 바로 갱신.
    public void OnClickWeapon(int weaponId)
    {
        if (_presenter == null || EquipManager.Instance == null) return;

        // 모델에서 선택(프리팹)
        _presenter.SelectWeapon(weaponId);

        // EquipManager가 단일 진실(Single Source of Truth)
        EquipManager.Instance.SetPlayerWeapon(_presenter.CurrentWeapon);
        EquipManager.Instance.EquipPlayerWeapon(); // 여기서 인스턴스 생성/복원/브릿지/이벤트까지 처리

        // UI 트리도 "실제 장착 인스턴스" 기준으로 맞춘다
        var inst = EquipManager.Instance.CurrentWeaponInstance;
        var tree = inst != null ? inst.PerksTree : null;

        if (_perkPanelUI != null)
            _perkPanelUI.ApplyPerksTree(tree);

        if (!UIManager.Instance.IsUIPopUp())
            UIManager.Instance.OpenUI(_weaponSelectMainUI);

        if (_titleText != null && weaponId >= 0 && weaponId < _allWeaponData.Length)
            _titleText.text = $"{_allWeaponData[weaponId].name.ToUpper()}";
    }
}
