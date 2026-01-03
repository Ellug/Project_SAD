using System.Collections;
using UnityEngine;

public class MultiLaserBombingPattern : PatternBase
{
    [Header("레이저 포격 설정")]
    [SerializeField, Tooltip("레이저 프리팹")] private ParticleSystem _laserParticlePrefab;
    [SerializeField, Tooltip("레이저 포격 갯수")] private int _laserCount = 5;
    [SerializeField, Tooltip("레이저 포격 반경")] private float _bombingRadius = 3.0f;
    [SerializeField, Tooltip("레이저 포격 간격")] private float _intervalBetweenLasers = 0.1f;
    [SerializeField, Tooltip("레이저 폭발 판정 범위")] private float _explosionRange = 1.5f;
    [SerializeField, Tooltip("데미지")] private float _damage = 10f;

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Vector3 finalCenterPos;
        if (_useFixedSpawnPoint)
        {
            finalCenterPos = transform.position;
        }
        else
        {
            finalCenterPos = _warningTransform != null ? _warningTransform.position : transform.position;
        }

        RemoveWarning();

        for (int i = 0; i < _laserCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _bombingRadius;
            Vector3 firePos = finalCenterPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            SpawnLaser(firePos);
            PlayPatternSound(PatternEnum.LaserBombing);
            CheckDamage(firePos);

            yield return new WaitForSeconds(_intervalBetweenLasers);
        }
    }

    private void SpawnLaser(Vector3 position)
    {
        if (_laserParticlePrefab == null) return;

        ParticleSystem laser = PoolManager.Instance.Spawn(_laserParticlePrefab, position, Quaternion.identity);
        if (laser != null)
        {
            laser.Clear();
            laser.Play();
        }
    }

    private void CheckDamage(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _explosionRange);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                if (hit.TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_damage);
                }
                break;
            }
        }
    }

    protected override void CleanupPattern()
    {
    }
}