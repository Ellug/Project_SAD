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
    [Tooltip("그을음 프리팹")] public BurnDecal _BurnDecalPrefab;
    [Tooltip("그을음 프리팹 생성주기")] public float _DecalTime = 5f;

    [Header("플레이어 정보")]
    [Tooltip("플레이어")] public GameObject _player;
    [Tooltip("데미지")] public float _Dmg = 5f;
    [Tooltip("데미지 딜레이")] public float _DmgDelayTime = 0.5f;

    private bool _delayCheck = true;
    private bool _dmgDelayCheck = true;
    private LayerMask _layerMask;
    private ParticleSystem[] _sparkChildren;

    private void Awake()
    {
        _layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Wall"));
    }

    private void OnEnable()
    {
        _delayCheck = true;
        _dmgDelayCheck = true;
    }

    private void OnDisable()
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.zero);
        }

        if (_laserHitObject != null) _laserHitObject.SetActive(false);
    }

    private void Start()
    {
        if (_sparkParticle != null)
        {
            _sparkChildren = _sparkParticle.GetComponentsInChildren<ParticleSystem>();
        }
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(_firePoint.position, _firePoint.forward, out hit, _maxLaserDistance, _layerMask))
        {
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, hit.point);

            Vector3 hitPos = hit.point + hit.normal * _hitParticleOffset;
            Quaternion hitRot = Quaternion.LookRotation(hit.normal);

            if (_sparkParticle != null)
            {
                _sparkParticle.transform.position = hitPos;
                _sparkParticle.transform.rotation = hitRot;
            }

            if (_laserHitObject != null)
            {
                _laserHitObject.transform.position = hitPos;
                _laserHitObject.transform.rotation = hitRot;
                if (!_laserHitObject.activeSelf) _laserHitObject.SetActive(true);
            }

            if (hit.collider.CompareTag("Player"))
            {
                if (_dmgDelayCheck)
                {
                    if (_player != null && _player.TryGetComponent<PlayerModel>(out var player))
                    {
                        player.TakeDamage(_Dmg);
                        _dmgDelayCheck = false;
                        StartCoroutine(DmgDelayRoutine());
                    }
                }
            }
            else
            {
                EmitAllChildren(_sparkChildren, 3);

                if (_delayCheck)
                {
                    PoolManager.Instance.Spawn(_BurnDecalPrefab, hit.point, hitRot);
                    _delayCheck = false;
                    StartCoroutine(DecalDelayRoutine());
                }
            }
        }
        else
        {
            _lineRenderer.SetPosition(0, _firePoint.position);
            _lineRenderer.SetPosition(1, _firePoint.position + _firePoint.forward * _maxLaserDistance);

            if (_laserHitObject != null && _laserHitObject.activeSelf)
                _laserHitObject.SetActive(false);
        }
    }

    private void EmitAllChildren(ParticleSystem[] systems, int count)
    {
        if (systems == null) return;
        foreach (var ps in systems)
        {
            if (ps != null) ps.Emit(count);
        }
    }

    IEnumerator DecalDelayRoutine()
    {
        yield return new WaitForSeconds(_DecalTime);
        _delayCheck = true;
    }

    IEnumerator DmgDelayRoutine()
    {
        yield return new WaitForSeconds(_DmgDelayTime);
        _dmgDelayCheck = true;
    }

    public void Init(GameObject target)
    {
        _player = target;
    }
}