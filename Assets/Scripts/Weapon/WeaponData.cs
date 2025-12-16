using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/Weapon")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private int _weaponId;
    public GameObject WeaponPrefab => _weaponPrefab;
    public int WeaponId => _weaponId;

    [Header("Normal Attack")]
    public float attack;
    public float attackSpeed;
    public int projectileCount;
    public float projectileAngle;
    public float projectileRange;
    public float projectileSpeed;
    public PlayerBullet projectilePrefab;

    [Header("Special Attack")]
    public float SpecialAttack;
    public float SpecialAttackSpeed;
    public float SpecialAttackDelay;
    public int SpecialProjectileCount;
    public float SpecialProjectileAngle;
    public float SpecialProjectileRange;
    public float SpecialProjectileSpeed;
    public PlayerBullet SpecialProjectilePrefab;
}
