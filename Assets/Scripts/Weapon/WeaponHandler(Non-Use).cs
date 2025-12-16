using UnityEngine;

public enum Stage
{
    Stage1,
    Stage2,
    Stage3
}
public enum Peck
{
    None,
    Left,
    Right
}
[System.Serializable]
public struct StagePeck
{
    public Stage Stage;
    public Peck Peck;

    public StagePeck(Stage stage, Peck peck)
    {
        Stage = stage;
        Peck = peck;
    }
}

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private LoadOutUI _ui;
    [SerializeField] private WeaponController _weaponController;
    [SerializeField] private PlayerModel _playerModel;

    [Header("Weapon Datas")]
    [SerializeField] private WeaponData _rifleData;
    [SerializeField] private WeaponData _snipeData;
    [SerializeField] private WeaponData _shotgunData;

    private Weapon _currentWeapon;
    private WeaponData _currentWeaponData;
    private StagePeck _currentStagePeck = new StagePeck(Stage.Stage1, Peck.None);

    public Weapon CurrentWeapon => _currentWeapon;
    public WeaponData CurrentWeaponData => _currentWeaponData;

    private void Awake()
    {
        if (_ui != null)
        {
            //_ui.OnWeaponSelected += HandleWeaponSelected;
            //TODO : LoadOutUI 에서 선택받은 값 구독
            //_ui.OnStagePeckSelected += HandleStagePeckSelected;
        }
    }

    void Start()
    {
        // SetCurrentWeapon(_currentWeapon, _currentWeaponData);
    }

    private void OnDestroy()
    {
        if (_ui != null)
        {
            //_ui.OnWeaponSelected -= HandleWeaponSelected;
            //_ui.OnStagePeckSelected += HandleStagePeckSelected;
        }
    }

    private void HandleWeaponSelected(Weapon weapon)
    {
        WeaponData data = null;

        switch (weapon)
        {
            case Weapon.Rifle:
                data = _rifleData;
                break;

            case Weapon.Snipe:
                data = _snipeData;
                break;

            case Weapon.Shotgun:
                data = _shotgunData;
                break;
        }
        if (data == null)
        {
            Debug.LogError("HandlerError : 무기 데이터가 비어있습니다.");
            return;
        }
        WeaponData selectWeaponData = Instantiate(data);
        //TODO : (하드코딩예정 부분이라 편히 사용..) peck 계산될 부분


        


        //WeaponManager.Instance.SetWeapon(weapon, selectWeaponData);
        //_weaponController.Init(selectWeaponData);
        //_playerModel.SetWeapon(_weaponController);
    }


    private void SetCurrentWeapon(Weapon weapon, WeaponData data)
    {
        if(data == null)
        {
            return;
        }
        _currentWeapon = weapon;
        _currentWeaponData = data;

        switch (weapon)
        {
            case Weapon.Rifle: _rifleData = data; break;
            case Weapon.Snipe: _snipeData = data; break;
            case Weapon.Shotgun: _shotgunData = data; break;
        }
        _weaponController.Init(data);
        //_playerModel.SetWeapon(_weaponController);
    }
}