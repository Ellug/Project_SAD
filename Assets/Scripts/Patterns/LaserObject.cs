using UnityEngine;
using System.Collections;
public class LaserObject : MonoBehaviour
{
    private bool Moving = false;
    private bool Rotate = false;
    [Tooltip("오브젝트 이동 속도")][SerializeField] float _MoveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float _LifeTime = 5f;
    [Tooltip("오브젝트 회전 속도")][SerializeField] float _RotationSpeed = 5f;
    private Vector3 TargetPosition;
    private float UpPosition;
    private float UnderPosition;

    private void FixedUpdate()
    {
        if (Moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, _MoveSpeed * Time.deltaTime);

            if (transform.position.y == UpPosition) 
            {
                ActivateLaser();
                StartCoroutine(DeActivateObject());
                Moving = false;
            }

            if (transform.position.y == UnderPosition)
            {
                DeActivateLaser();
                StopCoroutine(DeActivateObject());
                Moving = false;
            }
        }
        if (Rotate) 
        {
            transform.Rotate(0, _RotationSpeed * Time.deltaTime, 0);
        }
    }



    public void ActivateObject()
    {
        transform.rotation = Quaternion.identity;
        UpPosition = this.transform.localScale.y / 2;
        Moving = true;
        TargetPosition = new Vector3(this.transform.position.x, UpPosition, this.transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(_LifeTime);
        UnderPosition = -(this.transform.transform.localScale.y / 2);
        Moving = true;
        DeActivateLaser();
        TargetPosition = new Vector3(this.transform.position.x, UnderPosition, this.transform.position.z);
    }

    private void ActivateLaser() 
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        Rotate = true;
    }

    private void DeActivateLaser() 
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        Rotate = false;
    }
}
