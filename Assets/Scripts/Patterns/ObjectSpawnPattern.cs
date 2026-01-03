using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnPattern : PatternBase
{
    [Header("장애물 오브젝트 설정")]
    [SerializeField, Tooltip("수동 등록할 장애물 오브젝트들")] private List<MovingObject> _movingObjects = new List<MovingObject>();

    [Header("오브젝트 이동 및 물리 설정")]
    [SerializeField, Tooltip("오브젝트 이동 속도")] private float _moveSpeed = 5f;
    [SerializeField, Tooltip("오브젝트 활성화 시간")] private float _lifeTime = 5f;
    [SerializeField, Tooltip("오브젝트 상승 좌표")] private float _upPosition = 0.5f;
    [SerializeField, Tooltip("오브젝트 하강 좌표")] private float _underPosition = -0.6f;

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        foreach (var movingObj in _movingObjects)
        {
            if (movingObj != null)
            {
                movingObj.Init(_moveSpeed, _lifeTime, _upPosition, _underPosition);
                movingObj.ActivateObject();
            }
        }

        PlayPatternSound(PatternEnum.ObjectSpawn);

        yield break;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        foreach (var movingObj in _movingObjects)
        {
            if (movingObj != null)
            {
                movingObj.DeactivateObject();
            }
        }
    }
}