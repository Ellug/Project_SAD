using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _turret;

    public Transform Body => _body;
    public Rigidbody Rb => _rb;

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
}