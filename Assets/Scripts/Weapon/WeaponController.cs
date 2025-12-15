using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private WeaponData _weaponData;

    public void Init(WeaponData weaponData)
    {
        _weaponData = weaponData;
    }
    
    public void Attack()
    {
        FireProjectile();
    }

    public void SpecialAttack()
    {

    }

    //불릿 정보 줄거
    private void FireProjectile() 
    {
        Vector3 spawnPos = transform.position + transform.forward * 0.5f;

        PlayerBullet bullet = Instantiate(_weaponData.projectilePrefab, spawnPos, transform.rotation);
        bullet.Init(_weaponData, this.transform);
    }
}
