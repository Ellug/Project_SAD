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
    private Vector3 _aimAt;

    private PlayerInput _playerInput;
    private Vector3 _dodgeDir;
    private float _dodgeRemainDist;
    private bool _isAttackHold;

    public event Action interactionObject;

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

        HandleAim();

        Fire();
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
        if (ctx.started)
            _isAttackHold = true;
        else if (ctx.canceled)
            _isAttackHold = false;

        if (_model.CurrentWeapon is Rifle rifle)
            rifle.SetAttackHold(_isAttackHold);
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

        // 닷지시 특수공격 취소
        if (_model.IsOnSpecialAttack && _model.CurrentWeapon != null)
            _model.CurrentWeapon.CancelSpecialAttack();

        Vector3 inputDir = new(_moveInput.x, 0, _moveInput.y);
        if (inputDir.sqrMagnitude < 0.01f)
            inputDir = _view.Body.forward;

        _dodgeDir = inputDir.normalized;
        _dodgeRemainDist = _model.DodgeSpeed * _model.DodgeDuration;

        _model.StartDodge();
    }

    public void OnInteraction(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        interactionObject?.Invoke();
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        UIManager.Instance.TogglePause();
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

        if (_model.IsInDebuffSlow)
        {
            targetSpeed *= (1f - _model.DebuffSlowRate);
        }


        float newSpeed = Mathf.MoveTowards(curSpeed, targetSpeed, _model.AccelForce * Time.fixedDeltaTime);

        //공격이 확인되면 즉시 감속 처리
        if (_model.attackImpulse > 0f)
        {
            newSpeed = Mathf.Max(newSpeed - _model.AttackSlowRate, _model.AttackMinSpeed);
            _model.attackImpulse = 0f;
        }

        //공격중이면서 이동입력X
        if (_isAttackHold && dir.sqrMagnitude < 0.01f)
        {
            newSpeed = 0f;
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

        float moveDist = _model.DodgeSpeed * Time.fixedDeltaTime;
        moveDist = Mathf.Min(moveDist, _dodgeRemainDist);

        Vector3 move = _dodgeDir * moveDist;
        _view.Rb.MovePosition(_view.Rb.position + move);

        _dodgeRemainDist -= moveDist;

        if (_dodgeRemainDist <= 0f)
            _model.StopDodge();
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
        _aimAt = worldCursorPos - _view.transform.position;
        _aimAt.y = 0;

        _cameraController.SetAimDirection(_aimAt);

        if (_model.IsOnSpecialAttack) return;
        _view.RotateTurret(_aimAt);
    }

    private void Fire()
    {
        if (!_isAttackHold) return;
        if (_model.IsOnSpecialAttack) return;
        if (!_model.CanAttack) return;

        if (_model.CurrentWeapon != null && _model.CurrentWeapon.TryAttack())
            _model.StartAttack();
    }

    public void OpenCloseUI(bool isOpen)
    {
        if (isOpen)
            _playerInput.SwitchCurrentActionMap("UI");
        else
            _playerInput.SwitchCurrentActionMap("Player");
    }
}
