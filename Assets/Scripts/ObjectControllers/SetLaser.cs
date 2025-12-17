using UnityEngine;

public class SetLaser : MonoBehaviour
{
    [Tooltip("라인 렌더러")] public LineRenderer lineRenderer;
    [Tooltip("레이저 시작지점")] public Transform firePoint;
    [Tooltip("레이저 최대 거리")] public float maxLaserDistance = 50f;

    [Header("히트 이펙트")]
    [Tooltip("스파크 파티클")] public ParticleSystem sparkParticle;
    [Tooltip("레이저 히트 지점")] public GameObject laserHitObject;
    [Tooltip("히트 포인트 이격거리")] public float hitParticleOffset = 0.05f;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            sparkParticle.transform.position = hit.point + hit.normal * hitParticleOffset;
            laserHitObject.transform.position = sparkParticle.transform.position;

            sparkParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
            laserHitObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            if (!laserHitObject.activeSelf)
            {
                laserHitObject.SetActive(true);
            }

            sparkParticle.Emit(3);
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

















