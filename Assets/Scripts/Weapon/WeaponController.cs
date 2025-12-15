using UnityEngine;

public class WeaponController : MonoBehaviour , IWeapon
{
    private WeaponData _weaponData;

    public void Init(WeaponData weaponData)
    {
        _weaponData = weaponData;
    }
    
    public void Attack()
    {

    }

    public float Damage()
    {
        return _weaponData.attack;
    }

    public void SpecialAttack()
    {

    }

    private void Fireprojectile()
    {
        Instantiate(_weaponData.projectilePrefab, transform.position, transform.rotation);
    }
}
