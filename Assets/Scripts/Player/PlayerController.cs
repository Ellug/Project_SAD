using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerView _view;

    [Header("Camera Settings")]
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private float _cameraOffset = 6f;
    [SerializeField] private float _cameraSmooth = 2f;
    [SerializeField] private float _cameraDisMultiplier = 0.2f;

    private Camera _cam;
    private Plane _groundPlane;
    private Vector2 _moveInput;
    private Vector3 _dodgeDirection;
    private Vector3 _aimAt;

    private PlayerInput _playerInput;

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
        _model.CurrentWeapon?.Attack(_model);
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!_model.CanSpecialAttack) return;
        _model.StartSpecialAttack();
        _model.CurrentWeapon?.SpecialAttack(_model);
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
        }
    }

    public void AimAt(Vector3 worldCursorPos)
    {
        if (_model.IsOnSpecialAttack) return;

        _aimAt = worldCursorPos - _view.transform.position;
        _aimAt.y = 0;
        _view.RotateTurret(_aimAt);        
    }

    void LateUpdate()
    {
        // 카메라 업데이트
        UpdateCameraTarget(_aimAt);        
    }

    // 씨네머신 카메라 페이크 타겟 추적
    private void UpdateCameraTarget(Vector3 aimDir)
    {
        Vector3 playerPos = _view.transform.position;

        // 마우스까지의 거리
        float dist = aimDir.magnitude;
        float maxDist = _cameraOffset;
        float clampedDist = Mathf.Min(dist * _cameraDisMultiplier, maxDist);

        Vector3 dir = aimDir.normalized;

        Vector3 targetPos = playerPos + dir * clampedDist;
        targetPos.y = _cameraTarget.position.y;

        _cameraTarget.position = targetPos;
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
