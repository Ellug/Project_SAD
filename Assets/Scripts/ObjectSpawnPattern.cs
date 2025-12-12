using UnityEngine;

public class ObjectSpawnPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] PatternObject;
    public MovingObject[] MovingObject;
    private void Awake()
    {
        PatternObject = GameObject.FindGameObjectsWithTag("PatternObject");
    }

    public void ActivateObject()
    {
        MovingObject = new MovingObject[PatternObject.Length];
        for (int i = 0; i < PatternObject.Length; i++)
            MovingObject[i] = PatternObject[i].GetComponent<MovingObject>();

        for (int i = 0; i < MovingObject.Length; i++)
            MovingObject[i].ActivateObject();
    }

    protected override void PatternLogic()
    {
        
    }
}

