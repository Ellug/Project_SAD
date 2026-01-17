using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostLaserPattern : PatternBase
{
    [Header("레이저 오브젝트 설정")]
    [SerializeField, Tooltip("오브젝트 태그")] private GameObject[] _objectTag;
    [SerializeField, Tooltip("레이저 오브젝트 컴포넌트들")] public FrostLaserObject[] _laserObject;

    [Header("오브젝트 이동 및 물리 설정")]
    [SerializeField, Tooltip("오브젝트 이동 속도")] private float _moveSpeed = 5f;
    [SerializeField, Tooltip("오브젝트 활성화 시간")] private float _lifeTime = 5f;
    [SerializeField, Tooltip("오브젝트 회전 속도")] private float _rotationSpeed = 50f;
    [SerializeField, Tooltip("오브젝트 상승 좌표")] private float _upPosition = -0.2f;
    [SerializeField, Tooltip("오브젝트 하강 좌표")] private float _underPosition = -1.45f;

    [Header("레이저 세부 설정")]
    [SerializeField, Tooltip("레이저 최대 길이")] private float _maxLaserDistance = 50f;
    [SerializeField, Tooltip("히트 파티클 이격 거리")] private float _hitParticleOffset = 0.05f;
    [SerializeField, Tooltip("데칼 생성 텀")] private float _decalTime = 0.1f;

    [Header("데미지 및 디버프 설정")]
    [SerializeField, Tooltip("레이저 데미지")] private float _laserDmg = 5f;
    [SerializeField, Tooltip("데미지 딜레이")] private float _dmgDelay = 0.5f;
    [SerializeField, Tooltip("추위 데미지")] private float _coldDmg = 5f;
    [SerializeField, Tooltip("추위 지속시간")] private float _coldTime = 5f;
    [SerializeField, Tooltip("추위 틱")] private float _coldInterval = 1f;
        protected override void Awake()
    {
        base.Awake();
        _objectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public override void Init(GameObject target)
    {
        base.Init(target);
        _laserObject = new FrostLaserObject[_objectTag.Length];
        for (int i = 0; i < _objectTag.Length; i++)
        {
            _laserObject[i] = _objectTag[i].GetComponent<FrostLaserObject>();
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;
        PlayPatternSound(PatternEnum.FrostLaser);

        foreach (var laserObj in _laserObject)
        {
            if (laserObj != null)
            {
                laserObj.Init(_target, _moveSpeed, _lifeTime, _rotationSpeed, _upPosition, _underPosition);
                laserObj.SetLaserStats(_maxLaserDistance, _hitParticleOffset, _decalTime, _laserDmg, _dmgDelay, _coldDmg, _coldTime, _coldInterval);
                laserObj.ActivateObject();
            }
        }
        yield return new WaitForSeconds(_lifeTime);

        CleanupPattern();
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;
        foreach (var laserObj in _laserObject)
        {
            if (laserObj != null)
            {
                laserObj.DeactivateObject();
            }
        }
    }
}