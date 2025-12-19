using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private Transform _muzzlePos;
    [SerializeField] private GameObject _weaponSelectPanel;
    [SerializeField] private GameObject _perkSelectPanel;
    [SerializeField] private PerkSelectPanelUI _perkPanelUI;
    [SerializeField] private WeaponBase[] _allWeaponData;

    private WeaponPresenter _presenter;
    private Button[] _perkSelectList;
    private Button[] _weaponButtonList;
    private PerksTree _perksTree;

    void Start()
    {
        Init();

        WeaponModel model = new();
        model.Init(_allWeaponData);
        _presenter = new WeaponPresenter(model, this);
        _presenter.Init();
        _playerModel.SetWeapon(_presenter.CurrentWeapon);
    }

    // 무기 버튼과 특전 버튼을 모두 가져옴
    private void Init()
    {
        _weaponButtonList = _weaponSelectPanel.GetComponentsInChildren<Button>();
        _perkSelectList = _perkSelectPanel.GetComponentsInChildren<Button>();

        TextMeshProUGUI buttonText;
        foreach (Button button in _perkSelectList)
        {
            buttonText = button.transform.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "";
        }
    }

    // 무기를 선택하면 일어날 일.
    // 무기 정보도 가져오고 무기별 특전도 가져와야함.
    public void OnClickWeapon(int weaponId)
    {
        PanelInit();
        _presenter.SelectWeapon(weaponId);
        _weaponButtonList[weaponId].interactable = false;

        // GameManager에 프리팹 저장
        EquipManager.Instance.SetPlayerWeapon(_presenter.CurrentWeapon);

        // 로비에 있는 플레이어에게 무기 쥐어줌.
        if (_muzzlePos.childCount > 0)
        {
            for (int i = _muzzlePos.childCount - 1; i >= 0; i--)
            {
                Destroy(_muzzlePos.GetChild(i).gameObject);
            }
        }

        GameObject weapon = Instantiate(_presenter.CurrentWeapon.gameObject, _muzzlePos);

        var weaponInstance = weapon.GetComponent<WeaponBase>();
        _playerModel.SetWeapon(weaponInstance);

        // 무기의 PerksTree 변경을 GameManager에 저장
        BindPerksTree(weaponInstance.PerksTree);

        if (_perkPanelUI != null)
            _perkPanelUI.ApplyPerksTree(weaponInstance.PerksTree);
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

    // 각 무기를 클릭할 때마다 모든 버튼을 접근 -> 불필요한 연산이 있긴 함. 개선 여지?
    private void PanelInit()
    {
        foreach (Button item in _weaponButtonList)
            item.interactable = true;
    }
}
