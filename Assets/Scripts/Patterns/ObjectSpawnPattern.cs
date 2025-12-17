using UnityEngine;

public class ObjectSpawnPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] PatternObject;
    public MovingObject[] MovingObject;
    protected override void Awake()
    {
        base.Awake();
        PatternObject = GameObject.FindGameObjectsWithTag("Obstacle");
    }

    public void ActivateObjects()
    {
        for (int i = 0; i < MovingObject.Length; i++)
            MovingObject[i].ActivateObject();
    }

    protected override void PatternLogic()
    {
        ActivateObjects();
    }

    public override void Init(GameObject target)
    {
        MovingObject = new MovingObject[PatternObject.Length];
        for (int i = 0; i < PatternObject.Length; i++)
            MovingObject[i] = PatternObject[i].GetComponent<MovingObject>();
    }
}

