using UnityEngine;
using System.Collections;

public class SetFrostLaser : MonoBehaviour
{
    [Header("컴포넌트 및 프리팹")]
    [SerializeField] protected LineRenderer _lineRenderer;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected ParticleSystem _sparkParticle;
    [SerializeField] protected ParticleSystem _sparkParticle2;
    [SerializeField] protected GameObject _laserHitObject;
    [SerializeField] protected BurnDecal _BurnDecalPrefab;

    protected float _maxLaserDistance;
    protected float _hitParticleOffset;
    protected float _DecalTime;
    protected GameObject _Player;
    protected float _Dmg;
    protected float _DmgDelayTime;
    protected float _ColdDmg;
    protected float _ColdTime;
    protected float _ColdInterval;

    protected bool _delayCheck = true;
    protected bool _dmgDelayCheck = true;
    protected LayerMask _layerMask;
    protected ParticleSystem[] _spark1Children;
    protected ParticleSystem[] _spark2Children;

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
            _spark1Children = _sparkParticle.GetComponentsInChildren<ParticleSystem>();
        if (_sparkParticle2 != null)
            _spark2Children = _sparkParticle2.GetComponentsInChildren<ParticleSystem>();
    }

    public void SetStats(GameObject player, float dist, float offset, float dTime, float dmg, float delay, float cDmg, float cTime, float cInterval)
    {
        _Player = player;
        _maxLaserDistance = dist;
        _hitParticleOffset = offset;
        _DecalTime = dTime;
        _Dmg = dmg;
        _DmgDelayTime = delay;
        _ColdDmg = cDmg;
        _ColdTime = cTime;
        _ColdInterval = cInterval;
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

            UpdateEffectTransform(_sparkParticle, hitPos, hitRot);
            UpdateEffectTransform(_sparkParticle2, hitPos, hitRot);

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
                    if (_Player != null && _Player.TryGetComponent<PlayerModel>(out var player))
                    {
                        player.TakeDamage(_Dmg);
                        player.ColdDebuff(_ColdDmg, _ColdTime, _ColdInterval);
                        _dmgDelayCheck = false;
                        StartCoroutine(DmgDelayRoutine());
                    }
                }
            }
            else
            {
                EmitAllChildren(_spark1Children, 3);
                EmitAllChildren(_spark2Children, 3);
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

    protected void UpdateEffectTransform(ParticleSystem ps, Vector3 pos, Quaternion rot)
    {
        if (ps != null) { ps.transform.position = pos; ps.transform.rotation = rot; }
    }

    protected void EmitAllChildren(ParticleSystem[] systems, int count)
    {
        if (systems == null) return;
        foreach (var ps in systems) { if (ps != null) ps.Emit(count); }
    }

    protected IEnumerator DecalDelayRoutine() { yield return new WaitForSeconds(_DecalTime); _delayCheck = true; }
    protected IEnumerator DmgDelayRoutine() { yield return new WaitForSeconds(_DmgDelayTime); _dmgDelayCheck = true; }
}