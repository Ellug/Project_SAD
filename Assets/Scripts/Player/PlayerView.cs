using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _turret;

    public Transform Body => _body;
    public Rigidbody Rb => _rb;

    private Collider _playerCollider;
    private LayerMask _obstacleMask = 1 << 6;
    private const int MAX_HITS = 16;
    private readonly Collider[] _overlapHits = new Collider[MAX_HITS];

    void Awake()
    {
        _playerCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        ResolveOverlap();
    }

    public void Move(Vector3 velocity)
    {
        _rb.linearVelocity = velocity;
    }

    // 본체 회전
    public void RotateBody(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            _body.rotation = Quaternion.LookRotation(dir);
    }

    // 포신 회전
    public void RotateTurret(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            _turret.rotation = Quaternion.LookRotation(dir);
    }

    public void ResolveOverlap()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            _playerCollider.bounds.center,
            _playerCollider.bounds.extents.magnitude,
            _overlapHits,
            _obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        // 최대 반복 탐색
        const int MAX_ITERS = 6;

        for (int iter = 0; iter < MAX_ITERS; iter++)
        {
            bool resolvedAny = false;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _overlapHits[i];
                if (hit == _playerCollider) continue;

                if (!Physics.ComputePenetration(
                        _playerCollider, transform.position, transform.rotation,
                        hit, hit.transform.position, hit.transform.rotation,
                        out Vector3 dir, out float dist))
                    continue;

                Vector3 pushDir = new(dir.x, 0f, dir.z);

                // 중앙처럼 XZ가 거의 0이면, 표면 기준 방향을 재생성
                if (pushDir.sqrMagnitude < 1e-6f)
                {
                    Vector3 closest = hit.ClosestPoint(transform.position);
                    pushDir = transform.position - closest;
                    pushDir.y = 0f;
                }

                // 그래도 0이면 몸 방향
                if (pushDir.sqrMagnitude < 1e-6f)
                {
                    pushDir = Body.forward;
                    pushDir.y = 0f;
                }

                // 밀어내기
                pushDir.Normalize();
                _rb.position += pushDir * dist;

                resolvedAny = true;
                break;
            }

            if (!resolvedAny) break;
        }
    }

}