using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class WeaponController : MonoBehaviour
{
    private WeaponData _weaponData;
    private bool _isPrewarmed = false;

    public void Init(WeaponData weaponData)
    {
        _weaponData = weaponData;

        if (!_isPrewarmed)
        {
            PoolManager.Instance.Prewarm(_weaponData.projectilePrefab, 20);
            _isPrewarmed = true;
        }
    }
    
    public void Attack(PlayerModel player)
    {
        FireProjectile();
        player.StartAttackSlow();
    }
    public void SpecialAttack(PlayerModel player)
    {
        StartCoroutine(CoSpecialAttack(player));
    }

    private IEnumerator CoSpecialAttack(PlayerModel player)
    {
        yield return StartCoroutine(CoBeforeSpecialAttack(player));
        FireProjectile();
        yield return StartCoroutine(CoAfterSpecialAttack(player));
    }
    private IEnumerator CoBeforeSpecialAttack(PlayerModel player)
    {
        player.SetSpecialAttackState(true);
        yield return new WaitForSeconds(_weaponData.SpecialAttackBeforeDelay);
    }

    private IEnumerator CoAfterSpecialAttack(PlayerModel player)
    {
        yield return new WaitForSeconds(_weaponData.SpecialAttackAfterDelay);
        player.SetSpecialAttackState(false);
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

        PlayerBullet bullet = PoolManager.Instance.Spawn(_weaponData.projectilePrefab, pos, rot);
        bullet.Init(_weaponData, transform);
    }
}
