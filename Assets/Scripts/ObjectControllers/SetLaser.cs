using UnityEngine;

public class SetLaser : MonoBehaviour
{
    [Tooltip("레이저 라인렌더러")] public LineRenderer lineRenderer;
    [Tooltip("레이저 시작지점")]  public Transform firePoint;
    [Tooltip("레이저 최대 길이")] public float maxLaserDistance = 50f;
    [Tooltip("레이저 히트 지점")] public GameObject laserHitObject;
    [Tooltip("레이저 히트 이펙트 이격 거리")] public float hitParticleOffset = 0.1f;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            if (laserHitObject != null)
            {
                Vector3 offsetPosition = hit.point + (-firePoint.forward * hitParticleOffset);

                laserHitObject.transform.position = offsetPosition;

                Vector3 lookDirection = firePoint.position - laserHitObject.transform.position;
                laserHitObject.transform.rotation = Quaternion.LookRotation(lookDirection);

                if (!laserHitObject.activeSelf)
                {
                    laserHitObject.SetActive(true);
                }
            }
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.forward * maxLaserDistance);

            if (laserHitObject != null && laserHitObject.activeSelf)
            {
                laserHitObject.SetActive(false);
            }
        }
    }
}