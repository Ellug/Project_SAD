using UnityEngine;
using System.Collections;

public class FrostLaserObject : MonoBehaviour
{
    private bool _moving = false;
    private bool _rotate = false;
    [Tooltip("오브젝트 이동 속도")][SerializeField] float _moveSpeed = 5f;
    [Tooltip("오브젝트 활성화 시간")][SerializeField] float _lifeTime = 5f;
    [Tooltip("오브젝트 회전 속도")][SerializeField] float _rotationSpeed = 5f;

    private Vector3 _targetPosition;
    private const float _upPosition = -0.2f;
    private const float _underPosition = -1.45f;
    private Coroutine _actionCoroutine;
    private GameObject Player;

    private SetFrostLaser[] _lasers;

    private void Awake()
    {
        _lasers = GetComponentsInChildren<SetFrostLaser>(true);
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
                _actionCoroutine = StartCoroutine(DeActivateObject());
                _moving = false;
            }

            if (Mathf.Approximately(transform.position.y, _underPosition))
            {
                DeActivateLaser();
                if (_actionCoroutine != null) StopCoroutine(_actionCoroutine);
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
        transform.rotation = Quaternion.identity;
        _moving = true;
        _targetPosition = new Vector3(transform.position.x, _upPosition, transform.position.z);
    }

    IEnumerator DeActivateObject()
    {
        yield return new WaitForSeconds(_lifeTime);
        _moving = true;
        _targetPosition = new Vector3(transform.position.x, _underPosition, transform.position.z);
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
                    laser.Init(Player);
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
        Player = target;
    }
}