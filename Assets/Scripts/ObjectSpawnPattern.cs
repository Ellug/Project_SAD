using System.Collections;
using UnityEngine;
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
public class ObjectSpawnPattern : PatternBase
{
    [Tooltip("오브젝트 태그")][SerializeField] GameObject[] PatternObject;
    public MovingObject[] MovingObject;
<<<<<<< Updated upstream
    private void Awake()
    {
        PatternObject = GameObject.FindGameObjectsWithTag("PatternObject");
    }

    public void ActivateObject()
    {
        MovingObject = new MovingObject[PatternObject.Length];
        for (int i = 0; i < PatternObject.Length; i++)
=======

    private void Awake()
    {
        PatternObject = GameObject.FindGameObjectsWithTag("PatternObject"); 
    }
    public void ActivateObject() 
    {
        MovingObject = new MovingObject[PatternObject.Length];
        for(int i =0; i < PatternObject.Length; i++)
>>>>>>> Stashed changes
            MovingObject[i] = PatternObject[i].GetComponent<MovingObject>();

        for (int i = 0; i < MovingObject.Length; i++)
            MovingObject[i].ActivateObject();
    }
}
<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
