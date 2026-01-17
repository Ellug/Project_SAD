using System.Collections;
using UnityEngine;

public class IceAreaPattern : PatternBase
{
    [Header("냉기 장판 생성 설정")]
    [SerializeField, Tooltip("냉기 장판 프리팹")] private IceArea _IceAreaPrefab;

    [Header("냉기 설정")]
    [SerializeField, Tooltip("실제 타격 판정 범위 (지름)")] private float _IceAreaRange = 5.0f;
    [SerializeField, Tooltip("냉기 지속 시간")] private float _IceAreaLifeTime = 5.0f;
    [SerializeField, Tooltip("냉기 데미지")] private float _Dmg = 10.0f;
    [SerializeField, Tooltip("데미지 딜레이")] private float _DmgDelay = 0.5f;

    [Header("디버프 설정")]
    [SerializeField, Tooltip("냉기 지속시간")] private float _ColdDebuffTime = 2.0f;
    [SerializeField, Tooltip("냉기 데미지")] private float _ColdDmg = 5.0f;
    [SerializeField, Tooltip("냉기 틱")] private float _TickInterval = 0.5f;

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        yield return StartCoroutine(ShowWarning());

        Vector3 spawnPos;
        if (_useFixedSpawnPoint)
        {
            spawnPos = transform.position;
        }
        else
        {
            spawnPos = _warningTransform != null ? _warningTransform.position : transform.position;
        }

        RemoveWarning();
        SpawnIceArea(spawnPos);
    }

    private void SpawnIceArea(Vector3 position)
    {
        if (_IceAreaPrefab == null) return;

        PlayPatternSound(PatternEnum.IceArea);
        IceArea ice = PoolManager.Instance.Spawn(_IceAreaPrefab, position, Quaternion.identity);

        if (ice != null)
        {
            ice.Init(_target, _IceAreaRange, _IceAreaLifeTime, _Dmg, _DmgDelay, _ColdDmg, _ColdDebuffTime, _TickInterval);
        }
    }

    protected override void CleanupPattern()
    {
    }
}