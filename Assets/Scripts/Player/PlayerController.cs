using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerView _view;
    [SerializeField] private PlayerCameraController _cameraController;

    [Header("PlayerLaser Settings")]
    [SerializeField] private PlayerLaser playerLaser;

    private Camera _cam;
    private Plane _groundPlane;
    private Vector2 _moveInput;
    private Vector3 _dodgeDirection;
    private Vector3 _aimAt;

    private PlayerInput _playerInput;

    public event Action _interactionObject;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        _cam = Camera.main;
    }

    void Update()
    {
        _model.UpdateTimer(Time.deltaTime);
        _model.UpdateDodge(Time.deltaTime);
        _model.UpdateAttackSlow(Time.deltaTime);

        HandleAim();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleDodgeState();
    }

    // Input Actions - New Input System
    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_model.IsOnSpecialAttack) return;
        if (!_model.CanAttack) return;

        _model.StartAttack();
        _model.CurrentWeapon?.Attack();
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!_model.CanSpecialAttack) return;

        _model.StartSpecialAttack();
        _model.CurrentWeapon?.SpecialAttack();
    }

    public void OnDodge(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!_model.CanDodge) return;
        if (_model.IsOnSpecialAttack) return;

        _dodgeDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;

        if (_dodgeDirection.sqrMagnitude < 0.01f)
            _dodgeDirection = _view.Body.transform.forward;

        _model.StartDodge();
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _interactionObject?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        GameManager.Instance.TogglePause();
    }

    // Movement
    private void HandleMovement()
    {
        if (_model.IsDodging) return;
        if (_model.IsOnSpecialAttack) return;

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

        //공격이 확인되면 감속까지 추가 계산
        if (_model.IsOnAttack)
        {
            newSpeed = ApplyAttackSlow(_model.AttackSlowRate, newSpeed);
        }

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
            if (playerLaser != null) 
            {
                playerLaser.CursorPoint = hitPoint;
                if (_model.CurrentWeapon != null)
                    playerLaser.maxLaserDistance = _model.FinalStats.Weapon.ProjectileRange;
            }
        }
    }

    public void AimAt(Vector3 worldCursorPos)
    {
        if (_model.IsOnSpecialAttack) return;

        _aimAt = worldCursorPos - _view.transform.position;
        _aimAt.y = 0;

        _view.RotateTurret(_aimAt);
        _cameraController.SetAimDirection(_aimAt);
    }

    //Move할 속도에 SlowRate % 만큼 감속
    private float ApplyAttackSlow(float OnAttackSlowRate, float newSpeed)
    {
        return (1 - OnAttackSlowRate) * newSpeed;
    }

    public void OpenCloseUI(bool isOpen)
    {
        if (isOpen) 
            _playerInput.SwitchCurrentActionMap("UI");
        else
            _playerInput.SwitchCurrentActionMap("Player");
    }
}
