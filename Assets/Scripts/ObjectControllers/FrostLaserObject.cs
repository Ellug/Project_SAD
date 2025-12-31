using UnityEngine;
using System.Collections;

public class FrostLaserObject : MonoBehaviour
{
    [Tooltip("오브젝트 이동 속도")][SerializeField] float _moveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float _lifeTime = 5f;
    [Tooltip("오브젝트 회전 속도")][SerializeField] float _rotationSpeed = 5f;
    [Tooltip("오브젝트 업 포지션")][SerializeField] float _upPosition = -0.2f;
    [Tooltip("오브젝트 다운 포지션")][SerializeField] float _underPosition = -1.45f;

    private bool _moving = false;
    private bool _rotate = false;
    private Vector3 _targetPosition; 
    private Coroutine _actionCoroutine;
    private GameObject _player;
    private SetFrostLaser[] _lasers;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _lasers = GetComponentsInChildren<SetFrostLaser>(true);
        _initialRotation = transform.rotation;
    }

    private void Start()
    {
        Vector3 initPos = transform.position;
        initPos.y = _underPosition;
        transform.position = initPos;
    }

    private void FixedUpdate()
    {
        if (_moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.fixedDeltaTime);

            if (Mathf.Approximately(transform.position.y, _upPosition))
            {
                ActivateLaser();
                _actionCoroutine = StartCoroutine(DeActivateRoutine());
                _moving = false;
            }

            if (Mathf.Approximately(transform.position.y, _underPosition))
            {
                DeActivateLaser();
                _moving = false;
            }
        }

        if (_rotate)
        {
            transform.Rotate(0, _rotationSpeed * Time.fixedDeltaTime, 0);
        }
    }

    public void ActivateObject()
    {
        if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);

        transform.rotation = _initialRotation;
        _targetPosition = new Vector3(transform.position.x, _upPosition, transform.position.z);
        _moving = true;
    }

    public void DeactivateObject()
    {
        if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);

        _rotate = false;
        _targetPosition = new Vector3(transform.position.x, _underPosition, transform.position.z);
        _moving = true;
    }

    private IEnumerator DeActivateRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        DeactivateObject();
    }

    private void ActivateLaser()
    {
        if (_lasers != null)
        {
            foreach (var laser in _lasers)
            {
                if (laser != null)
                {
                    laser.gameObject.SetActive(true);
                    laser.Init(_player);
                }
            }
        }
        _rotate = true;
    }

    private void DeActivateLaser()
    {
        if (_lasers != null)
        {
            foreach (var laser in _lasers)
            {
                if (laser != null)
                {
                    laser.gameObject.SetActive(false);
                }
            }
        }
        _rotate = false;
    }

    public void Init(GameObject target)
    {
        _player = target;
    }
}