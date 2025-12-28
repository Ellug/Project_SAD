using UnityEngine;

public class LaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] _objectTag;
    public LaserObject[] _laserObject;
    private GameObject Player;

    protected override void Awake()
    {
        base.Awake();
        _objectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public void ActivateObjects()
    {
        for (int i = 0; i < _laserObject.Length; i++) 
        {
            _laserObject[i].ActivateObject();
            _laserObject[i].Init(Player);
        }         
    }

    protected override void PatternLogic()
    {
        ActivateObjects();
    }

    public override void Init(GameObject target)
    {
        _laserObject = new LaserObject[_objectTag.Length];
        for (int i = 0; i < _objectTag.Length; i++)
            _laserObject[i] = _objectTag[i].GetComponent<LaserObject>();
        Player = target;
    }
}
