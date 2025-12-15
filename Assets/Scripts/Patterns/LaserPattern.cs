using UnityEngine;

public class LaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] _objectTag;
    public LaserObject[] _laserObject;

    private void Awake()
    {
        _objectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public void ActivateObject()
    {
        _laserObject = new LaserObject[_laserObject.Length];
        for (int i = 0; i < _laserObject.Length; i++)
            _laserObject[i] = _laserObject[i].GetComponent<LaserObject>();

        for (int i = 0; i < _laserObject.Length; i++)
            _laserObject[i].ActivateObject();
    }

    protected override void PatternLogic()
    {

    }

    public override void Init(GameObject target)
    {

    }
}
