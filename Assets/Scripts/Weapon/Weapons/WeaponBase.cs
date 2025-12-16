using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] private WeaponData _weaponData;

    public int GetWeaponId()
    {
        return _weaponData.WeaponId;
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
        int count = Mathf.Max(1, _weaponData.projectileCount);
        float totalAngle = _weaponData.projectileAngle;

        Vector3 baseDir = transform.forward;
        baseDir.y = 0f;
        baseDir.Normalize();

        Vector3 spawnPos = transform.position + baseDir * 0.5f;

        // 각도, 숫자로 산탄 계산
        if (count == 1 || totalAngle == 0f)
        {
            SpawnBullet(spawnPos, baseDir);
            return;
        }

        float halfAngle = totalAngle * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);

            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * baseDir;
            SpawnBullet(spawnPos, dir);
        }
    }

    private void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        PlayerBullet bullet = Instantiate(_weaponData.projectilePrefab, pos, rot);
        bullet.Init(_weaponData, transform);
    }
}
