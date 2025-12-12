using UnityEngine;
using System.Collections;

public class MovingObject : PatternBase
{
    private bool Moving = false;
    [Tooltip("오브젝트 이동 속도")][SerializeField] float MoveSpeed = 0;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float LifeTime = 5f;
    private Vector3 TargetPosition;

    private void FixedUpdate()
    {
<<<<<<< Updated upstream
        if (Moving)            
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);
=======
        if (Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime);
>>>>>>> Stashed changes

            if (transform.position == TargetPosition)
            {
                StartCoroutine(DeActivateObject());
                Moving = false;
                StopCoroutine(DeActivateObject());
            }
        }
    }

<<<<<<< Updated upstream


=======
>>>>>>> Stashed changes
    public void ActivateObject()
    {
        float UpPosition = this.transform.localScale.y / 2;
        Moving = true;
        TargetPosition = new Vector3(this.transform.position.x, UpPosition, this.transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(LifeTime);
        float UnderPosition = -(this.transform.transform.localScale.y / 2);
        Moving = true;
        TargetPosition = new Vector3(this.transform.position.x, UnderPosition, this.transform.position.z);
<<<<<<< Updated upstream
    }

}
=======

    }
}
>>>>>>> Stashed changes
