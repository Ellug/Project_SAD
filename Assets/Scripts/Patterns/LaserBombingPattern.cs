using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MultiLaserBombingPattern : PatternBase
{
    [Header("경고 장판")]
    [SerializeField][Tooltip("경고 장판 프리팹")] private ParticleSystem _warningAreaPrefab;
    [SerializeField][Tooltip("경고 장판 추적 시간")] private float _chaseTime = 2.0f;     
    [SerializeField][Tooltip("경고 장판 고정 시간")] private float _readyTime = 0.5f;     
    [SerializeField][Tooltip("경고 장판 이격 거리")] private float _chaseOffset = 0.2f;   

    [Header("레이저 포격 설정")]
    [SerializeField][Tooltip("레이저 프리팹")] private ParticleSystem _laserParticlePrefab; 
    [SerializeField][Tooltip("레이저 포격 갯수")] private int _laserCount = 5;                 
    [SerializeField][Tooltip("레이저 포격 반경")] private float _bombingRadius = 3.0f;         
    [SerializeField][Tooltip("레이저 포격 간격")] private float _intervalBetweenLasers = 0.1f; 
    [SerializeField][Tooltip("레이저 폭발 판정 범위")] private float _explosionRange = 1.5f;       
    [SerializeField][Tooltip("데미지")] private float _damage = 10f;

    private PredictiveAim _predictiveAim;
    private GameObject _player;
    private ParticleSystem _currentWarning;
    private bool Chase = false;

    public override void Init(GameObject target)
    {
        _player = target;
        _predictiveAim = GameObject.FindAnyObjectByType<PredictiveAim>();
    }

    protected override void PatternLogic()
    {
        StartCoroutine(ExecutePattern());
    }

    private IEnumerator ExecutePattern()
    {
        if (_warningAreaPrefab == null || _predictiveAim == null) yield break;

        _currentWarning = PoolManager.Instance.Spawn(_warningAreaPrefab, _predictiveAim.PredictiveAimCalc(_chaseOffset), Quaternion.identity);
        Chase = true;

        _currentWarning.Clear();
        _currentWarning.Play();

        float elapsed = 0;
        while (elapsed < _chaseTime)
        {
            if (_currentWarning != null)
                _currentWarning.transform.position = _predictiveAim.PredictiveAimCalc(_chaseOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Chase = false;
        Vector3 finalCenterPos = _currentWarning.transform.position;

        yield return new WaitForSeconds(_readyTime);

        if (_currentWarning != null)
            if (_currentWarning != null)
            {
                _currentWarning.Stop();
                PoolManager.Instance.Despawn(_currentWarning.gameObject);
                _currentWarning = null;
            }

        StartCoroutine(FireMultiLasers(finalCenterPos));
    }

    private IEnumerator FireMultiLasers(Vector3 centerPos)
    {
        for (int i = 0; i < _laserCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * _bombingRadius;
            Vector3 firePos = centerPos + new Vector3(randomCircle.x, 0, randomCircle.y);

            SpawnLaser(firePos);

            CheckDamage(firePos);

            yield return new WaitForSeconds(_intervalBetweenLasers);
        }
    }

    private void SpawnLaser(Vector3 position)
    {
        if (_laserParticlePrefab == null) return;

        ParticleSystem laser = Instantiate(_laserParticlePrefab, position, Quaternion.identity);

        var main = laser.main;
        main.stopAction = ParticleSystemStopAction.Destroy;

        laser.Play();
    }

    private void CheckDamage(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, _explosionRange);
        foreach (var hit in colliders)
        {
            if (hit.gameObject == _player)
            {
                if (_player.TryGetComponent<PlayerModel>(out var player))
                {
                    player.TakeDamage(_damage);
                }
                break;
            }
        }
    }
}