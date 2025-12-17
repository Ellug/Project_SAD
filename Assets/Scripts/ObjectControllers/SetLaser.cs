using UnityEngine;
using System.Collections;

public class SetLaser : MonoBehaviour
{
    [Tooltip("라인 렌더러")] public LineRenderer lineRenderer;
    [Tooltip("레이저 시작지점")] public Transform firePoint;
    [Tooltip("레이저 최대 거리")] public float maxLaserDistance = 50f;

    [Header("히트 이펙트")]
    [Tooltip("스파크 파티클")] public ParticleSystem sparkParticle;
    [Tooltip("레이저 히트 지점")] public GameObject laserHitObject;
    [Tooltip("히트 포인트 이격거리")] public float hitParticleOffset = 0.05f;
    [Tooltip("그을음  프리팹")] public BurnDecal BurnDecalPrefab;
    [Tooltip("그을음  프리팹 생성주기")] public float _DecalTime = 5f;

    private bool DelayCheck = true;

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, maxLaserDistance))
        {
            //라인 렌더러의 시작지점과 끝지점을 설정
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hit.point);

            //레이저 히트 지점, 스파크 튀기는 지점 설정
            sparkParticle.transform.position = hit.point + hit.normal * hitParticleOffset;
            laserHitObject.transform.position = sparkParticle.transform.position;

            //회전각을 초기화 
            sparkParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
            laserHitObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            if (!laserHitObject.activeSelf)
            {
                laserHitObject.SetActive(true);
            }

            sparkParticle.Emit(3);
            StartCoroutine(DelayTime());
            if (DelayCheck) 
            {
                BurnDecal dec = PoolManager.Instance.Spawn(BurnDecalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                DelayCheck = false;
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

    IEnumerator DelayTime() 
    {
        yield return new WaitForSeconds(_DecalTime);
        DelayCheck = true;
    }
}

















