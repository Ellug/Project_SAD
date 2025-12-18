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

    private void Start()
    {
        Vector3 initPos = transform.position;
        initPos.y = -((transform.localScale.y / 2) + 0.1f);
        transform.position = initPos;
    }

    // 장애물 오브젝트는 맵 아래에 항상 존재함. 그래서 패턴 수행 명령이 아닐 때도
    // FixedUpdate는 계속 돌아갈 것임. -> 패턴 수행 됐을 때만 실행하면 안될까?
    private void FixedUpdate()
    {
        if (Moving)            
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, MoveSpeed * Time.fixedDeltaTime);

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
        UpPosition = transform.localScale.y / 2;
        Moving = true;
        TargetPosition = new Vector3(transform.position.x, UpPosition, transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(LifeTime);
        UnderPosition = -((transform.localScale.y / 2) + 0.1f);
        Moving = true;
        TargetPosition = new Vector3(transform.position.x, UnderPosition, transform.position.z);
    }
}