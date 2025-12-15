using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour
{
    private bool Moving = false;
    [Tooltip("오브젝트 이동 속도")][SerializeField] float MoveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float LifeTime = 5f;
    private Vector3 TargetPosition;
    private float UpPosition;
    private float UnderPosition;

    private void FixedUpdate()
    {
        if (Moving)            
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.deltaTime);

            if (transform.position.y == UpPosition)
            {
                StartCoroutine(DeActivateObject());
                Moving = false;
            }

            if (transform.position.y == UnderPosition)
            {
                StopCoroutine(DeActivateObject());
                Moving = false;
            }
        }
    }



    public void ActivateObject()
    {
        UpPosition = this.transform.localScale.y / 2;
        Moving = true;
        TargetPosition = new Vector3(this.transform.position.x, UpPosition, this.transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(LifeTime);
        UnderPosition = -(this.transform.transform.localScale.y / 2);
        Moving = true;
        TargetPosition = new Vector3(this.transform.position.x, UnderPosition, this.transform.position.z);
    }
}