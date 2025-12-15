using UnityEngine;

public class LaserPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] ObjectTag;
    public LaserObject[] LaserObject;
    private void Awake()
    {
        ObjectTag = GameObject.FindGameObjectsWithTag("LaserObject");
    }

    public void ActivateObjects()
    {
        LaserObject = new LaserObject[ObjectTag.Length];
        for (int i = 0; i < ObjectTag.Length; i++)
            LaserObject[i] = ObjectTag[i].GetComponent<LaserObject>();

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
