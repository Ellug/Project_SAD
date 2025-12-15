using Unity.VisualScripting;
using UnityEngine;

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

    public Weapon CurrentWeapon => _currentWeapon;
    public WeaponData CurrentWeaponData => _currentWeaponData;

    private void Awake()
    {
        _ui.OnWeaponSelected += HandleWeaponSelected;

        SetCurrentWeapon(_currentWeapon, _currentWeaponData);
    }

    private void OnDestroy()
    {
        _ui.OnWeaponSelected -= HandleWeaponSelected;
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

        WeaponManager.Instance.SetWeapon(weapon, data);
        _weaponController.Init(data);
        _playerModel.SetWeapon(_weaponController);
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
        _playerModel.SetWeapon(_weaponController);
    }
}