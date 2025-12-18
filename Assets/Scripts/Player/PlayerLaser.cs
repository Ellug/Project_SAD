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

    void Update()
    {
        //레이저 출발지점과 마우스 커서 포인트까지의 거리
        float cursorDistance = Vector3.Distance(firePoint.position, CursorPoint);

        // 커서가 사거리 안에 있으면 커서까지, 아니면 최대 사거리까지
        float finalDistance = Mathf.Min(cursorDistance, maxLaserDistance);
        Vector3 targetPoint = firePoint.position + (firePoint.forward * finalDistance);
        bool isHittingSomething = false;
        Vector3 hitNormal = Vector3.up;

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            if (hit.distance < finalDistance)
            {
                targetPoint = hit.point;
                hitNormal = hit.normal;
                isHittingSomething = true;
            }
            else if (cursorDistance <= hit.distance)
            {
                targetPoint = CursorPoint;
                isHittingSomething = true; 
            }
        }
        else
        {
            if (cursorDistance <= maxLaserDistance)
            {
                targetPoint = CursorPoint;
                isHittingSomething = true;
            }
        }

        UpdateLaserVisuals(targetPoint, hitNormal, isHittingSomething);
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