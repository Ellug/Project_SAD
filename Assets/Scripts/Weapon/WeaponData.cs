using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/Weapon")]
public class WeaponData : ScriptableObject
{
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
    public float SpecialAttackBeforeDelay;
    public float SpecialAttackAfterDelay;
    public int SpecialProjectileCount;
    public float SpecialProjectileAngle;
    public float SpecialProjectileRange;
    public float SpecialProjectileSpeed;
    public PlayerBullet SpecialProjectilePrefab;
}
