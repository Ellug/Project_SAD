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
    private Button[] _weaponButtonList;
    private PerksTree _perksTree;
    private TextMeshProUGUI _titleText;
    private int _selectedWeapon;

    void Start()
    {
        Init();

        WeaponModel model = new();
        model.Init(_allWeaponData);
        _presenter = new WeaponPresenter(model, this);
        _presenter.Init();
        _playerModel.SetWeapon(_presenter.CurrentWeapon);

        UIManager.Instance.AllUIClosed += InitSelectedWeapon;
        _selectedWeapon = -1;
    }

    private void OnDestroy()
    {
        UIManager.Instance.AllUIClosed -= InitSelectedWeapon;
    }

    // 무기 버튼과 특전 버튼을 모두 가져옴
    private void Init()
    {
        _weaponButtonList = _weaponList.GetComponentsInChildren<Button>();
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
        // 같은 무기를 여러번 클릭하면 로직을 실행하지 않는다.
        if (_selectedWeapon == weaponId)
            return;
        _selectedWeapon = weaponId;

        _presenter.SelectWeapon(weaponId);

        // EquipManager에 프리팹 저장
        EquipManager.Instance.SetPlayerWeapon(_presenter.CurrentWeapon);

        // 로비에 있는 플레이어에게 무기 시험을 위해 장착 시켜줌.
        if (_muzzlePos.childCount > 0)
            for (int i = _muzzlePos.childCount - 1; i >= 0; i--)
                Destroy(_muzzlePos.GetChild(i).gameObject);

        GameObject weapon = Instantiate(_presenter.CurrentWeapon.gameObject, _muzzlePos);
        var weaponInstance = weapon.GetComponent<WeaponBase>();
        _playerModel.SetWeapon(weaponInstance);

        // 무기의 PerksTree 변경을 GameManager에 저장
        BindPerksTree(weaponInstance.PerksTree);

        // 특전 노드 생성
        if (_perkPanelUI != null)
            _perkPanelUI.ApplyPerksTree(weaponInstance.PerksTree);

        // 특전 UI가 켜져 있지 않다면 열어야 함.
        if (!UIManager.Instance.IsUIPopUp())
            UIManager.Instance.OpenUI(_weaponSelectMainUI);

        // 특전 UI에 선택된 무기 정보를 주입함.
        _titleText.text = $"{_allWeaponData[weaponId].name.ToUpper()}";
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

    public void InitSelectedWeapon()
    {
        _selectedWeapon = -1;
    }
}
