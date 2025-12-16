using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private Transform _muzzlePos;
    [SerializeField] private GameObject _weaponSelectPanel;
    [SerializeField] private GameObject _peckSelectPanel;
    [SerializeField] private WeaponBase[] _allWeaponData;

    private WeaponPresenter _presenter;
    private Button[] _peckSelectList;
    private Button[] _weaponButtonList;

    private void Start()
    {
        Init();
        WeaponModel model = new WeaponModel();
        model.Init(_allWeaponData);
        _presenter = new WeaponPresenter(model, this);
        _presenter.Init();
        _playerModel.SetWeapon(_presenter.CurrentWeapon);
    }

    // 무기 버튼과 특전 버튼을 모두 가져옴
    private void Init()
    {
        _weaponButtonList = _weaponSelectPanel.GetComponentsInChildren<Button>();
        _peckSelectList = _peckSelectPanel.GetComponentsInChildren<Button>();

        TextMeshProUGUI buttonText;
        foreach (Button button in _peckSelectList)
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

        if (_muzzlePos.childCount > 0)
        {
            for (int i = _muzzlePos.childCount - 1; i >= 0; i--)
            {
                Destroy(_muzzlePos.GetChild(i).gameObject);
            }
        }
        Instantiate(_presenter.CurrentWeapon.gameObject, _muzzlePos);
        _playerModel.SetWeapon(_presenter.CurrentWeapon);
    }

    // 특.전.처.리를 조지려면 어떻게 해야할까요?
    // 특전 다 모으기? 아니요~
    public void OnClickPeck()
    {

    }

    // 각 무기를 클릭할 때마다
    // 모든 버튼을 접근한다. -> 불필요한 연산이 있긴 함.
    private void PanelInit()
    {
        foreach (Button item in _weaponButtonList)
            item.interactable = true;
    }
}
