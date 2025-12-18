using Unity.VisualScripting;
using UnityEngine;

public class PlayerLaser : MonoBehaviour
{
    [Tooltip("라인 렌더러")] public LineRenderer lineRenderer;
    [Tooltip("레이저 시작지점")] public Transform firePoint;
    [Tooltip("레이저 최대 거리")] public float maxLaserDistance = 50f;

    [Header("히트 이펙트")]
    [Tooltip("레이저 히트 지점")] public GameObject laserHitObject;
    [Tooltip("히트 포인트 이격거리")] public float hitParticleOffset = 0.05f;

    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            //라인 렌더러의 시작지점과 끝지점을 설정
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            laserHitObject.transform.position = hit.point + hit.normal * hitParticleOffset;

            //회전각을 초기화 
            laserHitObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            if (!laserHitObject.activeSelf)
            {
                laserHitObject.SetActive(true);
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
