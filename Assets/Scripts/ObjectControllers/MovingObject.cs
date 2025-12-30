using UnityEngine;
using System.Collections;

public class MovingObject : MonoBehaviour
{
    [Tooltip("오브젝트 이동 속도")][SerializeField] private float _moveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] private float _lifeTime = 5f;

    private bool _isMoving = false;
    private Vector3 _targetPosition;
    private float _upPosition;
    private float _underPosition;
    private Coroutine _actionCoroutine;

    private void Start()
    {
        _underPosition = -((transform.localScale.y / 2f) + 0.1f);
        Vector3 initPos = transform.position;
        initPos.y = _underPosition;
        transform.position = initPos;
    }

    // 장애물 오브젝트는 맵 아래에 항상 존재함. 그래서 패턴 수행 명령이 아닐 때도
    // FixedUpdate는 계속 돌아갈 것임. -> 패턴 수행 됐을 때만 실행하면 안될까?

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

        _upPosition = transform.localScale.y / 2f;
        _targetPosition = new Vector3(transform.position.x, _upPosition, transform.position.z);
        _isMoving = true;
    }

    public void DeactivateObject()
    {
        if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);

        _underPosition = -((transform.localScale.y / 2f) + 0.1f);
        _targetPosition = new Vector3(transform.position.x, _underPosition, transform.position.z);
        _isMoving = true;
    }

    private IEnumerator DeActivateRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        DeactivateObject();
    }
}