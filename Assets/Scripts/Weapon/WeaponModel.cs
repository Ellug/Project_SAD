using System.Collections.Generic;

// 모든 무기들의 정보와 특전의 정보를 가지고 있는
// 무기고 역할을 하게될 WeaponModel
public class WeaponModel
{
    // 딕셔너리를 통해 무기 id를 통해 무기를 받음.
    private Dictionary<int, WeaponBase> _armory;

    public WeaponBase GetWeapon(int weaponId)
    {
        return _armory[weaponId];
    }

    public void Init(WeaponBase[] armory)
    {
        // 딕셔너리에 모든 무기를 등록함.
        _armory = new Dictionary<int, WeaponBase>();

        foreach (WeaponBase weapon in armory) 
        {
            _armory[weapon.GetWeaponId()] = weapon;
        }
    }
}
