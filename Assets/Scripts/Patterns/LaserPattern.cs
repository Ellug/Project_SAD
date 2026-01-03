using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPattern : PatternBase
{
    [Header("레이저 오브젝트 설정")]
    [SerializeField, Tooltip("수동 등록할 레이저 오브젝트들")] private List<LaserObject> _laserObjects = new List<LaserObject>();

    [Header("오브젝트 이동 및 물리 설정")]
    [SerializeField, Tooltip("오브젝트 이동 속도")] private float _moveSpeed = 5f;
    [SerializeField, Tooltip("오브젝트 활성화 시간")] private float _lifeTime = 5f;
    [SerializeField, Tooltip("오브젝트 회전 속도")] private float _rotationSpeed = 5f;
    [SerializeField, Tooltip("오브젝트 상승 좌표")] private float _upPosition = -0.2f;
    [SerializeField, Tooltip("오브젝트 하강 좌표")] private float _underPosition = -1.45f;

    [Header("레이저 세부 설정")]
    [SerializeField, Tooltip("레이저 최대 길이")] private float _maxLaserDistance = 50f;
    [SerializeField, Tooltip("히트 파티클 이격 거리")] private float _hitParticleOffset = 0.05f;
    [SerializeField, Tooltip("데칼 생성 텀")] private float _decalTime = 5f;

    [Header("데미지 설정")]
    [SerializeField, Tooltip("레이저 데미지")] private float _damage = 5f;
    [SerializeField, Tooltip("데미지 딜레이")] private float _damageDelay = 0.5f;

    public override void Init(GameObject target)
    {
        base.Init(target);
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        foreach (var laserObj in _laserObjects)
        {
            if (laserObj != null)
            {
                laserObj.Init(_target, _moveSpeed, _lifeTime, _rotationSpeed, _upPosition, _underPosition);
                laserObj.SetLaserStats(_maxLaserDistance, _hitParticleOffset, _decalTime, _damage, _damageDelay);
                laserObj.ActivateObject();
            }
        }

        yield break;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
        foreach (var laserObj in _laserObjects)
        {
            if (laserObj != null) laserObj.DeactivateObject();
        }
    }
}