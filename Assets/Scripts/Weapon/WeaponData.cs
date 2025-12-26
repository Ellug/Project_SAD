using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/Weapon")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private int _weaponId;
    [SerializeField] private AudioClip _fireClip;
    [SerializeField] private AudioClip _reloadClip;
    public GameObject WeaponPrefab => _weaponPrefab;
    public AudioClip FireClip => _fireClip;
    public AudioClip ReloadClip => _reloadClip;
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
    public float SpecialAttackBeforeDelay;
    public float SpecialAttackAfterDelay;
    public int SpecialProjectileCount;
    public float SpecialProjectileAngle;
    public float SpecialProjectileRange;
    public float SpecialProjectileSpeed;
    public PlayerBullet SpecialProjectilePrefab;
}
