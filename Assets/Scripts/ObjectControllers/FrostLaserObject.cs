using UnityEngine;
using System.Collections;

public class FrostLaserObject : MonoBehaviour
{
    protected float _moveSpeed;
    protected float _lifeTime;
    protected float _rotationSpeed;
    protected float _upPosition;
    protected float _underPosition;

    protected bool _moving = false;
    protected bool _rotate = false;
    protected Vector3 _targetPosition;
    protected Coroutine _actionCoroutine;
    protected GameObject _player;
    protected SetFrostLaser[] _lasers;
    protected Quaternion _initialRotation;

    private void Awake()
    {
        _lasers = GetComponentsInChildren<SetFrostLaser>(true);
        _initialRotation = transform.rotation;
    }

    public void Init(GameObject target, float moveSpeed, float lifeTime, float rotSpeed, float up, float under)
    {
        _player = target;
        _moveSpeed = moveSpeed;
        _lifeTime = lifeTime;
        _rotationSpeed = rotSpeed;
        _upPosition = up;
        _underPosition = under;

        // 초기 위치 설정
        Vector3 initPos = transform.position;
        initPos.y = _underPosition;
        transform.position = initPos;
    }

    public void SetLaserStats(float dist, float offset, float dTime, float dmg, float delay, float cDmg, float cTime, float cInterval)
    {
        if (_lasers == null) return;
        foreach (var laser in _lasers)
        {
            if (laser != null)
                laser.SetStats(_player, dist, offset, dTime, dmg, delay, cDmg, cTime, cInterval);
        }
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

    protected IEnumerator DeActivateRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        DeactivateObject();
    }

    protected void ActivateLaser()
    {
        if (_lasers != null)
        {
            foreach (var laser in _lasers)
            {
                if (laser != null) laser.gameObject.SetActive(true);
            }
        }
        _rotate = true;
    }

    protected void DeActivateLaser()
    {
        if (_lasers != null)
        {
            foreach (var laser in _lasers)
            {
                if (laser != null) laser.gameObject.SetActive(false);
            }
        }
        _rotate = false;
    }
}