using UnityEngine;

public class FrostLaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] _objectTag;
    public FrostLaserObject[] _laserObject;
    private GameObject Player;

    protected override void Awake()
    {
        base.Awake();
        _objectTag = GameObject.FindGameObjectsWithTag("FrostLaserObject");
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
        _laserObject = new FrostLaserObject[_objectTag.Length];
        for (int i = 0; i < _objectTag.Length; i++)
            _laserObject[i] = _objectTag[i].GetComponent<FrostLaserObject>();
        Player = target;
    }
}
