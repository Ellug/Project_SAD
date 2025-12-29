using System.Collections;
using UnityEngine;

public class FrostLaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] private GameObject[] _objectTag;
    [Tooltip("레이저 오브젝트 컴포넌트들")][SerializeField] private FrostLaserObject[] _laserObject;

    private GameObject _player;

    protected override void Awake()
    {
        base.Awake();
        _objectTag = GameObject.FindGameObjectsWithTag("FrostLaserObject");
    }

    public override void Init(GameObject target)
    {
        _player = target;
        _laserObject = new FrostLaserObject[_objectTag.Length];
        for (int i = 0; i < _objectTag.Length; i++)
        {
            _laserObject[i] = _objectTag[i].GetComponent<FrostLaserObject>();
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        for (int i = 0; i < _laserObject.Length; i++)
        {
            if (_laserObject[i] != null)
            {
                _laserObject[i].ActivateObject();
                _laserObject[i].Init(_player);
            }
        }

        PlayPatternSound(PatternEnum.FrostLaser);

        yield break;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        for (int i = 0; i < _laserObject.Length; i++)
        {
            if (_laserObject[i] != null)
            {
                _laserObject[i].DeactivateObject();
            }
        }
    }
}