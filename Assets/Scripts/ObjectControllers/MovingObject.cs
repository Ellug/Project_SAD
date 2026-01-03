using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour
{
    protected float _moveSpeed;
    protected float _lifeTime;
    protected float _upPosition;
    protected float _underPosition;

    protected bool _isMoving = false;
    protected Vector3 _targetPosition;
    protected Coroutine _actionCoroutine;

    public void Init(float moveSpeed, float lifeTime, float up, float under)
    {
        _moveSpeed = moveSpeed;
        _lifeTime = lifeTime;
        _upPosition = up;
        _underPosition = under;

        Vector3 initPos = transform.position;
        initPos.y = _underPosition;
        transform.position = initPos;
    }

    private void FixedUpdate()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.fixedDeltaTime);

            if (Mathf.Approximately(transform.position.y, _upPosition))
            {
                _actionCoroutine = StartCoroutine(DeActivateRoutine());
                _isMoving = false;
            }

            if (Mathf.Approximately(transform.position.y, _underPosition))
            {
                _isMoving = false;
            }
        }
    }

    public void ActivateObject()
    {
        if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);

        _targetPosition = new Vector3(transform.position.x, _upPosition, transform.position.z);
        _isMoving = true;
    }

    public void DeactivateObject()
    {
        if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);

        _targetPosition = new Vector3(transform.position.x, _underPosition, transform.position.z);
        _isMoving = true;
    }

    protected IEnumerator DeActivateRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        DeactivateObject();
    }
}