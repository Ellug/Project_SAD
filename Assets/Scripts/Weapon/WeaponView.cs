using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private Transform _muzzlePos;
    [SerializeField] private GameObject _weaponList;
    [SerializeField] private GameObject _weaponSelectMainUI;
    // [SerializeField] private GameObject _perkSelectPanel;
    [SerializeField] private PerkSelectPanelUI _perkPanelUI;
    [SerializeField] private WeaponBase[] _allWeaponData;

    private WeaponPresenter _presenter;
    // private Button[] _perkSelectList;
    private PerksTree _perksTree;
    private TextMeshProUGUI _titleText;
    private WeaponSelectButtonUI[] _weaponBtns;

    void Start()
    {
        Init();

        // 기본 무기 표시(EquipManager 기준)
        if (EquipManager.Instance != null && EquipManager.Instance.Weapon != null)
            ApplyWeaponSelectedVisual(EquipManager.Instance.Weapon.GetWeaponId());
        else
            ApplyWeaponSelectedVisual(0); // fallback

        WeaponModel model = new();
        model.Init(_allWeaponData);
        _presenter = new WeaponPresenter(model, this);
        _presenter.Init();
    }

    // 무기 버튼과 특전 버튼을 모두 가져옴
    private void Init()
    {
        _titleText = _weaponSelectMainUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        // _perkSelectList = _perkSelectPanel.GetComponentsInChildren<Button>();

        // UI 담당자 : 아래 버튼 텍스트 초기화는 텍스트가 사라지면서 사용하지 않게됨.

        //TextMeshProUGUI buttonText;
        //foreach (Button button in _perkSelectList)
        //{
        //    buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();
        //    buttonText.text = "";
        //}
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

        BindPerksTree(tree);

        if (_perkPanelUI != null)
            _perkPanelUI.ApplyPerksTree(tree);

        if (!UIManager.Instance.IsUIPopUp())
            UIManager.Instance.OpenUI(_weaponSelectMainUI);

        if (_titleText != null && weaponId >= 0 && weaponId < _allWeaponData.Length)
            _titleText.text = $"{_allWeaponData[weaponId].name.ToUpper()}";

        ApplyWeaponSelectedVisual(weaponId);
    }

    private void BindPerksTree(PerksTree tree)
    {
        if (_perksTree != null)
            _perksTree.OnChanged -= OnPreviewPerksChanged;

        _perksTree = tree;

        if (_perksTree == null) return;

        _perksTree.OnChanged += OnPreviewPerksChanged;

        // 선택 UI 표시 갱신
        OnPreviewPerksChanged();
    }

    private void OnPreviewPerksChanged()
    {
        // 선택 상태를 GameManager에 저장
        EquipManager.Instance.SavePerksFrom(_perksTree);

        // 여기서 버튼 텍스트/하이라이트 갱신해도 됨. 현재는 
    }

    // 무기 선택
    private void CacheWeaponButtons()
    {
        _weaponBtns = _weaponList.GetComponentsInChildren<WeaponSelectButtonUI>(true);

        foreach (var wb in _weaponBtns)
        {
            if (wb == null || wb.button == null) continue;

            // 일반/선택 스프라이트 캐싱
            Image img = wb.button.targetGraphic as Image;
            wb.normalSprite = img != null ? img.sprite : null;
            wb.selectedSprite = wb.button.spriteState.selectedSprite; // 인스펙터에 넣어둔 Selected Sprite 재사용
        }
    }

    private void ApplyWeaponSelectedVisual(int selectedWeaponId)
    {
        if (_weaponBtns == null) return;

        foreach (var wb in _weaponBtns)
        {
            if (wb == null || wb.button == null) continue;

            Image img = wb.button.targetGraphic as Image;
            if (img == null) continue;

            bool isSelected = (wb.weaponId == selectedWeaponId);

            // 선택된 것만 selectedSprite, 나머지는 normalSprite
            img.sprite = isSelected && wb.selectedSprite != null ? wb.selectedSprite : wb.normalSprite;
        }
    }
}
