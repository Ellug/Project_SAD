using UnityEngine;
using System.Collections;

public class SetLaser : MonoBehaviour
{
    [Tooltip("라인 렌더러")] public LineRenderer _lineRenderer;
    [Tooltip("레이저 시작지점")] public Transform _firePoint;
    [Tooltip("레이저 최대 거리")] public float _maxLaserDistance = 50f;

    [Header("히트 이펙트")]
    [Tooltip("스파크 파티클")] public ParticleSystem _sparkParticle;
    [Tooltip("레이저 히트 지점")] public GameObject _laserHitObject;
    [Tooltip("히트 포인트 이격거리")] public float _hitParticleOffset = 0.05f;
    [Tooltip("그을음  프리팹")] public BurnDecal _BurnDecalPrefab;
    [Tooltip("그을음  프리팹 생성주기")] public float _DecalTime = 5f;

    [Header("플레이어 정보")]
    [Tooltip("플레이어")] public GameObject _Player;
    [Tooltip("데미지")] public float _Dmg = 5f;
    [Tooltip("데미지 딜레이")] public float _DmgDelayTime = 0.5f;

    private bool DelayCheck = true;
    private bool DmgDelayCheck = true;
    private LayerMask _layerMask;

    private void Awake()
    {
        _layerMask += 1 << LayerMask.NameToLayer("Player");
        _layerMask += 1 << LayerMask.NameToLayer("Wall");
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(_firePoint.position, _firePoint.forward, out hit, _maxLaserDistance, _layerMask))
        {
            //라인 렌더러의 시작지점과 끝지점을 설정
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, hit.point);

            //레이저 히트 지점, 스파크 튀기는 지점 설정
            _sparkParticle.transform.position = hit.point + hit.normal * _hitParticleOffset;
            _laserHitObject.transform.position = _sparkParticle.transform.position;

            //회전각을 초기화 
            _sparkParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
            _laserHitObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            if (!_laserHitObject.activeSelf)
            {
                _laserHitObject.SetActive(true);
            }

            StartCoroutine(DmgDelayTime());
            if (hit.collider.CompareTag("Player")) 
            {
                if (DmgDelayCheck) 
                {
                    _Player.TryGetComponent<PlayerModel>(out var player);
                    player.TakeDamage(_Dmg);
                    DmgDelayCheck = false;
                }
                return;
            }

            _sparkParticle.Emit(3);
            StartCoroutine(DelayTime());
            if (DelayCheck) 
            {
                BurnDecal dec = PoolManager.Instance.Spawn(_BurnDecalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                DelayCheck = false;
            }            
        }
        else
        {
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, _firePoint.position + _firePoint.forward * _maxLaserDistance);

            if (_laserHitObject != null && _laserHitObject.activeSelf)
            {
                _laserHitObject.SetActive(false);
            }
        }
    }

    IEnumerator DelayTime() 
    {
        yield return new WaitForSeconds(_DecalTime);
        DelayCheck = true;
    }

    IEnumerator DmgDelayTime()
    {
        yield return new WaitForSeconds(_DmgDelayTime);
        DmgDelayCheck = true;
    }
}

















