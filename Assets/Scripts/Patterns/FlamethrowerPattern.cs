using System.Collections;
using UnityEngine;

public class FlamethrowerPattern : PatternBase
{
    [Header("화염 방사 설정")]
    [Tooltip("화염방사 프리팹")][SerializeField] private Flamethrower _flamePrefab;
    [Tooltip("스폰 포인트")][SerializeField] private Transform _spawnPosition;

    [Header("경고 장판")]
    [Tooltip("경고 장판")][SerializeField] private ParticleSystem _WarningArea;
    [Tooltip("경고 장판 추적 시간")][SerializeField] private float _WarningTime = 1.5f;
    [Tooltip("경고 장판 지연 시간")][SerializeField] private float _WarningFNTime = 0.2f;
    [Tooltip("경고 장판의 가로 폭")][SerializeField] private float _WarningWidth = 10f;
    [Tooltip("경고 장판 길이")][SerializeField] private float _WarningLength = 15f;
    [Tooltip("장판 시작점 오프셋 비율 (0.5면 절반 전진)")][SerializeField] private float _WarningOffsetRate = 0.5f;

    private ParticleSystem _currentWarning;
    private Transform _warningTransform;
    private GameObject _target;
    private bool _isChasing = false;
    private bool _isFiring = false;
    private Flamethrower _activeFlame;
    private Quaternion _fireRotation;

    public override void Init(GameObject target) => _target = target;

    protected override void PatternLogic() => StartWarning();

    private void Update()
    {
        if (_target == null) return;

        if (_isChasing && _warningTransform != null)
        {
            UpdateRotation(_warningTransform);

            Vector3 forwardOffset = _warningTransform.forward * (_WarningLength * _WarningOffsetRate);
            _warningTransform.position = _spawnPosition.position + forwardOffset;

            _warningTransform.localScale = new Vector3(_WarningWidth, 1f, _WarningLength);
        }
    }

    private void UpdateRotation(Transform trans)
    {
        Vector3 dir = (_target.transform.position - _spawnPosition.position);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            trans.rotation = Quaternion.Slerp(trans.rotation, targetRot, Time.deltaTime * 50f);
        }
    }

    private void StartWarning()
    {
        if (_WarningArea == null) return;

        _currentWarning = PoolManager.Instance.Spawn(_WarningArea, _spawnPosition.position, Quaternion.identity);
        _warningTransform = _currentWarning.transform;

        _isChasing = true;
        _currentWarning.Clear();
        _currentWarning.Play();

        StartCoroutine(PatternSequence());
    }

    private IEnumerator PatternSequence()
    {
        yield return new WaitForSeconds(_WarningTime);
        _isChasing = false;

        yield return new WaitForSeconds(_WarningFNTime);

        if (_currentWarning != null)
        {
            _fireRotation = _warningTransform.rotation;
            _currentWarning.Stop();
            PoolManager.Instance.Despawn(_currentWarning.gameObject);
            _currentWarning = null;
            _warningTransform = null;
        }
        FireFlamethrower();
    }

    private void FireFlamethrower()
    {
        _activeFlame = PoolManager.Instance.Spawn(_flamePrefab, _spawnPosition.position, _fireRotation);

        if (_activeFlame != null)
        {
            foreach (ParticleSystem ps in _activeFlame.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);
            }

            _isFiring = true;
            _activeFlame.Init(_target);

            StartCoroutine(FiringRoutine());
        }
    }

    private IEnumerator FiringRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isFiring = false;
        _activeFlame = null;
    }

    protected override void Awake() => base.Awake();
}