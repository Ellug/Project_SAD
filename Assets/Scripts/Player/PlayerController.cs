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

    private Vector3 _dodgeDir;
    private float _dodgeRemainDist;
    private bool _isAttackHold;

    // 넉백
    private float _kbSkin = 0.02f; // 벽에 딱 붙는 걸 방지하는 여유값
    private bool _isKnockback;
    private Vector3 _kbDir;
    private float _kbRemainDist;
    private float _kbSpeed;

    public event Action interactionObject;

    void Start()
    {
        _groundPlane = new Plane(Vector3.up, Vector3.zero);
        _cam = Camera.main;
    }

    void Update()
    {
        _model.UpdateTimer(Time.deltaTime);
        _model.UpdateDodge(Time.deltaTime);

        Fire();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleDodgeState();
        HandleAim();

        HandleKnockbackState();
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

    // 넉백
    private void HandleKnockbackState()
    {
        // 이미 넉백 진행 중이면 "짧고 빠르게" 이동 처리
        if (_isKnockback)
        {
            float moveDist = _kbSpeed * Time.fixedDeltaTime;
            moveDist = Mathf.Min(moveDist, _kbRemainDist);

            if (moveDist > 0f)
            {
                // 이동 전 충돌 스윕으로 안전 거리만큼만 이동
                if (_view.Rb.SweepTest(_kbDir, out RaycastHit hit, moveDist + _kbSkin, QueryTriggerInteraction.Ignore))
                {
                    float safeDist = Mathf.Max(0f, hit.distance - _kbSkin);

                    if (safeDist > 0f)
                        _view.Rb.MovePosition(_view.Rb.position + _kbDir * safeDist);

                    // 벽에 막히면 즉시 종료(타격감)
                    _kbRemainDist = 0f;
                }
                else
                {
                    _view.Rb.MovePosition(_view.Rb.position + _kbDir * moveDist);
                    _kbRemainDist -= moveDist;
                }
            }

            if (_kbRemainDist <= 0f)
                _isKnockback = false;

            return;
        }

        // 넉백 요청이 있으면 시작
        if (_model.TryConsumeKnockbackRequest(out Vector3 dir, out float dist, out float duration))
        {
            // 닷지 중이면 넉백 무시(원하면 우선순위 조정 가능)
            if (_model.IsDodging) return;

            dir.y = 0f;
            if (dir.sqrMagnitude < 1e-6f) return;

            _kbDir = dir.normalized;
            _kbRemainDist = dist;
            _kbSpeed = dist / Mathf.Max(0.01f, duration);
            _isKnockback = true;

            // 넉백 시작 순간, 기존 관성 제거(원치 않으면 삭제)
            _view.Rb.linearVelocity = Vector3.zero;
        }
    }
}
