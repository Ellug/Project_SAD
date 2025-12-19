public class WeaponPresenter
{
    private WeaponModel _model;
    private WeaponView _view;

    private WeaponBase _currentWeapon;

    public WeaponBase CurrentWeapon => _currentWeapon;

    public WeaponPresenter(WeaponModel model, WeaponView view)
    {
        _model = model;
        _view = view;
        SelectWeapon(0);
    }

    public void Init()
    {
        _currentWeapon = _model.GetWeapon(0);
        PoolManager.Instance.Prewarm(_currentWeapon.WeaponData.projectilePrefab, 20);
        EquipManager.Instance.SetPlayerWeapon(_currentWeapon);
    }

    public void SelectWeapon(int weaponId)
    {
        // 해당 ID와 매칭된 무기 가져와
        _currentWeapon = _model.GetWeapon(weaponId);
        EquipManager.Instance.SetPlayerWeapon(_currentWeapon);
    }
}
