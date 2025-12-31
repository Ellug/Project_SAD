using System.Collections;
using UnityEngine;

public class ObjectSpawnPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] private GameObject[] _patternObject;
    [Tooltip("이동 오브젝트 컴포넌트들")] public MovingObject[] _movingObject;

    protected override void Awake()
    {
        base.Awake();
        _patternObject = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    public override void Init(GameObject target)
    {
        _movingObject = new MovingObject[_patternObject.Length];
        for (int i = 0; i < _patternObject.Length; i++)
        {
            _movingObject[i] = _patternObject[i].GetComponent<MovingObject>();
        }
    }

    protected override IEnumerator PatternRoutine()
    {
        _isPatternActive = true;

        for (int i = 0; i < _movingObject.Length; i++)
        {
            if (_movingObject[i] != null)
            {
                _movingObject[i].ActivateObject();
            }
        }

        PlayPatternSound(PatternEnum.ObjectSpawn);

        yield break;
    }

    protected override void CleanupPattern()
    {
        _isPatternActive = false;

        for (int i = 0; i < _movingObject.Length; i++)
        {
            if (_movingObject[i] != null)
            {
                _movingObject[i].DeactivateObject();
            }
        }
    }
}