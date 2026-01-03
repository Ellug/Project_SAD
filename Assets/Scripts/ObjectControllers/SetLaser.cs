using UnityEngine;
using System.Collections;

public class SetLaser : MonoBehaviour
{
    [Header("컴포넌트 및 프리팹")]
    [SerializeField] protected LineRenderer _lineRenderer;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected ParticleSystem _sparkParticle;
    [SerializeField] protected GameObject _laserHitObject;
    [SerializeField] protected BurnDecal _BurnDecalPrefab;

    protected float _maxLaserDistance;
    protected float _hitParticleOffset;
    protected float _DecalTime;
    protected GameObject _player;
    protected float _Dmg;
    protected float _DmgDelayTime;

    protected bool _delayCheck = true;
    protected bool _dmgDelayCheck = true;
    protected LayerMask _layerMask;
    protected ParticleSystem[] _sparkChildren;

    private void Awake()
    {
        _layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Wall"));
    }

    public void SetStats(GameObject player, float dist, float offset, float dTime, float dmg, float delay)
    {
        _player = player;
        _maxLaserDistance = dist;
        _hitParticleOffset = offset;
        _DecalTime = dTime;
        _Dmg = dmg;
        _DmgDelayTime = delay;
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
                    if (_player != null && _player.TryGetComponent<PlayerModel>(out var playerModel))
                    {
                        playerModel.TakeDamage(_Dmg);
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
            if (_laserHitObject != null && _laserHitObject.activeSelf) _laserHitObject.SetActive(false);
        }
    }

    protected void EmitAllChildren(ParticleSystem[] systems, int count)
    {
        if (systems == null) return;
        foreach (var ps in systems)
        {
            if (ps != null) ps.Emit(count);
        }
    }

    protected IEnumerator DecalDelayRoutine() { yield return new WaitForSeconds(_DecalTime); _delayCheck = true; }
    protected IEnumerator DmgDelayRoutine() { yield return new WaitForSeconds(_DmgDelayTime); _dmgDelayCheck = true; }
}