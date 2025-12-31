using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [Tooltip("라인 렌더러")] public LineRenderer lineRenderer;
    [Tooltip("레이저 시작지점")] public Transform firePoint;
    [Tooltip("레이저 최대 거리")] public float maxLaserDistance = 50f;

    [Header("히트 이펙트")]
    [Tooltip("레이저 히트 지점")] public GameObject laserHitObject;
    [Tooltip("히트 포인트 이격거리")] public float hitParticleOffset = 0.05f;

    [Tooltip("마우스 커서 포인트")] public Vector3 CursorPoint;

    private LayerMask _layerMask;

    private void Awake()
    {
        _layerMask += 1 << LayerMask.NameToLayer("Wall");
        _layerMask += 1 << LayerMask.NameToLayer("Enemy");
    }

    void FixedUpdate()
    {
        Vector3 targetPoint = firePoint.position + (firePoint.forward * maxLaserDistance);
        Vector3 hitNormal = Vector3.up;
        bool isHittingObject = false;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance, _layerMask))
        {
            targetPoint = hit.point;
            hitNormal = hit.normal;
            isHittingObject = true;
        }

        UpdateLaserVisuals(targetPoint, hitNormal, isHittingObject);
    }

    private void UpdateLaserVisuals(Vector3 endPoint, Vector3 normal, bool showEffect)
    {
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);

        if (showEffect)
        {
            if (!laserHitObject.activeSelf) laserHitObject.SetActive(true);

            laserHitObject.transform.position = endPoint + (normal * hitParticleOffset);
            laserHitObject.transform.rotation = Quaternion.LookRotation(normal);
        }
        else
        {
            if (laserHitObject.activeSelf) laserHitObject.SetActive(false);
        }
    }
}