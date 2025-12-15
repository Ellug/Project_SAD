using UnityEngine;

public class LaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] ObjectTag;
    public LaserObject[] LaserObject;
    private void Awake()
    {
        ObjectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public void ActivateObject()
    {
        LaserObject = new LaserObject[LaserObject.Length];
        for (int i = 0; i < LaserObject.Length; i++)
            LaserObject[i] = LaserObject[i].GetComponent<LaserObject>();

        for (int i = 0; i < LaserObject.Length; i++)
            LaserObject[i].ActivateObject();
    }

    protected override void PatternLogic()
    {

    }

    public override void Init(GameObject target)
    {

    }
}
