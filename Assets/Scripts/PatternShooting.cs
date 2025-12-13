using UnityEngine;

public class PatternShooting : PatternBase
{
    protected override void PatternLogic()
    {
        Debug.Log($"{gameObject.name} 패턴 실행!");
    }
}
