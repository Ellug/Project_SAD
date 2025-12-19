using UnityEngine;
using System.Collections;
public class LaserObject : MonoBehaviour
{
    private bool _moving = false;
    private bool _rotate = false;
    [Tooltip("오브젝트 이동 속도")][SerializeField] float _moveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float _lifeTime = 5f;
    [Tooltip("오브젝트 회전 속도")][SerializeField] float _rotationSpeed = 5f;
    private Vector3 _targetPosition;
    private float _upPosition;
    private float _underPosition;
    private Coroutine _actionCoroutine;

    private void Start()
    {
        Vector3 initPos = transform.position;
        initPos.y = -((transform.localScale.y / 2) + 0.1f);
        transform.position = initPos;
    }

    private void FixedUpdate()
    {
        if (_moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.fixedDeltaTime);

            if (transform.position.y == _upPosition) 
            {
                ActivateLaser();
                _actionCoroutine = StartCoroutine(DeActivateObject());
                _moving = false;
            }

            if (transform.position.y == _underPosition)
            {
                DeActivateLaser();
                StopCoroutine(_actionCoroutine);
                _moving = false;
            }
        }
        if (_rotate) 
        {
            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }
    }



    public void ActivateObject()
    {
        transform.rotation = Quaternion.identity;
        _upPosition = transform.localScale.y / 2;
        _moving = true;
        _targetPosition = new Vector3(transform.position.x, _upPosition, transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(_lifeTime);
        _underPosition = -((transform.localScale.y / 2) + 0.1f);
        _moving = true;
        DeActivateLaser();
        _targetPosition = new Vector3(transform.position.x, _underPosition, transform.position.z);
    }

    private void ActivateLaser() 
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        _rotate = true;
    }

    private void DeActivateLaser() 
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        _rotate = false;
    }
}
