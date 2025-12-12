using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerView _view;
    
    private Camera _cam;
    private Plane _groundPlane;
    private Vector2 _moveInput;
    private Vector3 _dodgeDirection;

    void Start()
    {
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        _cam = Camera.main;
    } 

    void Update()
    {
        _model.UpdateTimer(Time.deltaTime);
        _model.UpdateDodge(Time.deltaTime);
    }

    void FixedUpdate()
    {        
        HandleMovement();
        HandleDodgeState();
        HandleAim();
    }

    // Input Actions - New Input System
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _model.CurrentWeapon?.Attack();
    }
    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!_model.CanSpecialAttack) return;

        _model.StartSpecial();
        _model.CurrentWeapon?.SpecialAttack();
    }

    public void OnDodge(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!_model.CanDodge) return;

        _dodgeDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        if (_dodgeDirection.sqrMagnitude < 0.01f)
            _dodgeDirection = _view.Body.transform.forward;

        _model.StartDodge();
    }

    // Movement
    private void HandleMovement()
    {
        if (_model.IsDodging) return;

        Vector3 dir = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
        Vector3 currentVelocity = _view.Rb.linearVelocity;

        // 속도 방향 보간
        Vector3 newDir;
        if (dir.sqrMagnitude < 0.01f)
        {
            newDir = currentVelocity.normalized; // 입력이 없으면 현재 방향 유지
        }
        else
        {
            Vector3 curDir = (currentVelocity.sqrMagnitude < 0.01f) ? dir : currentVelocity.normalized;
            newDir = Vector3.Slerp(curDir, dir, Time.fixedDeltaTime * _model.RotSpeed);
        }

        // 가속 계산
        float curSpeed = currentVelocity.magnitude;
        float targetSpeed = dir.sqrMagnitude > 0.01f ? _model.MaxSpeed : 0f;

        float newSpeed = Mathf.MoveTowards(curSpeed, targetSpeed, _model.AccelForce * Time.fixedDeltaTime);

        // 최종 velocity 계산
        Vector3 finalVelocity = newDir * newSpeed;
        _view.Move(finalVelocity);
        _view.RotateBody(newDir);
    }


    // Dodge
    private void HandleDodgeState()
    {
        if (!_model.IsDodging) return;

        // View에 Dodge 속도 적용
        _view.Move(_dodgeDirection * _model.DodgeSpeed);
    }

    // Cursor Aim
    private void HandleAim()
    {
        Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (_groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            AimAt(hitPoint);
        }
    }

    public void AimAt(Vector3 worldCursorPos)
    {
        Vector3 aimDir = worldCursorPos - _view.transform.position;
        aimDir.y = 0;
        _view.RotateTurret(aimDir);
    }
}
