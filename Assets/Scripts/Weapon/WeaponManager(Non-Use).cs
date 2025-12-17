using UnityEngine;

public enum Weapon
{
    Rifle, Snipe, Shotgun
}

public class WeaponManager : SingletonePattern<WeaponManager>
{
    [field: SerializeField]
    public Weapon CurrentWeapon { get; private set; }

    [field: SerializeField]
    public WeaponData CurrentWeaponData { get; private set; }
    
    [Header("Default Weapon")]
    [SerializeField] private Weapon _defaultWeapon = Weapon.Rifle;
    [SerializeField] private WeaponData _defaultWeaponData;

    private void Start()
    {
        if (CurrentWeaponData == null)
            SetWeapon(_defaultWeapon, _defaultWeaponData);
    }

    public void SetWeapon(Weapon weapon, WeaponData weaponData)
    {
        CurrentWeapon = weapon;
        CurrentWeaponData = weaponData;
    }
}
