using System.Collections;
using UnityEngine;

public class LaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] private GameObject[] _objectTag;
    [Tooltip("레이저 오브젝트 컴포넌트들")] public LaserObject[] _laserObject;
    private GameObject _player;

    protected override void Awake()
    {
        base.Awake();
        _objectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public override void Init(GameObject target)
    {
        _player = target;
        _laserObject = new LaserObject[_objectTag.Length];
        for (int i = 0; i < _objectTag.Length; i++)
        {
            _laserObject[i] = _objectTag[i].GetComponent<LaserObject>();
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

       // PlayPatternSound(PatternEnum.Laser);

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