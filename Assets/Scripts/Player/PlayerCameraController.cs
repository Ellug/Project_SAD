using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float _cameraOffset = 6f;
    [SerializeField] private float _cameraDisMultiplier = 0.2f;

    private CinemachineCamera _virtualCamera;
    private Transform _cameraTarget;
    private Vector3 _aimDir;

    private void Awake()
    {
        EnsureVirtualCamera();
        EnsureCameraTarget();
        BindVirtualCamera();
    }

    void LateUpdate()
    {
        UpdateCameraTarget();
    }

    // Virtual Camera 확보
    private void EnsureVirtualCamera()
    {
        if (_virtualCamera != null) return;

        _virtualCamera = FindFirstObjectByType<CinemachineCamera>();

        if (_virtualCamera == null)
            Debug.LogError("PlayerCameraController: CinemachineVirtualCamera not found in scene.");
    }

    // CameraTarget 확보
    private void EnsureCameraTarget()
    {
        if (_cameraTarget != null) return;

        GameObject go = new("CameraTarget");
        _cameraTarget = go.transform;
        _cameraTarget.position = transform.position;
    }

    // Follow 바인딩
    private void BindVirtualCamera()
    {
        if (_virtualCamera == null || _cameraTarget == null) return;
        _virtualCamera.Follow = _cameraTarget;
    }

    /// PlayerController에서 에임 방향 전달
    public void SetAimDirection(Vector3 aimDir)
    {
        _aimDir = aimDir;
    }

    // 씨네머신 카메라 페이크 타겟 추적
    private void UpdateCameraTarget()
    {
        if (_aimDir.sqrMagnitude < 0.1f) return;

        Vector3 playerPos = transform.position;

        // 마우스까지의 거리
        float dist = _aimDir.magnitude;
        float clampedDist = Mathf.Min(dist * _cameraDisMultiplier, _cameraOffset);

        Vector3 dir = _aimDir.normalized;

        Vector3 targetPos = playerPos + dir * clampedDist;
        targetPos.y = _cameraTarget.position.y;

        _cameraTarget.position = targetPos;
    }
}
