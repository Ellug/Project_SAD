using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private BossController _boss;
    [SerializeField] private List<PhaseData> _phase;

    private PhaseData _curPhase;
    private int _phaseIndex;

    // 런타임 중 프리팹을 인스턴스한 오브젝트를 가진 배열
    private PatternBase[] _activeObjects;

    void Awake()
    {
        _boss._phaseChange += ChangePhase;
        _boss._takeCounterableAttack += TriggerCounter;
        _phaseIndex = 0;
        _curPhase = _phase[_phaseIndex];
    }

    void Start()
    {
        StartCurrentPhase();
    }

    void OnDestroy()
    {
        if (_boss != null)
        {
            _boss._phaseChange -= ChangePhase;
            _boss._takeCounterableAttack -= TriggerCounter;
        }
    }

    private void StartCurrentPhase()
    {
        _activeObjects = new PatternBase[_curPhase.Pattern.Count];
        // 담겨진 패턴들 모두 타이머 시작
        for (int i = 0; i < _activeObjects.Length; i++)
        {
            _activeObjects[i] = Instantiate(_curPhase.Pattern[i].gameObject).GetComponent<PatternBase>();
            _activeObjects[i].Init(_player);

            // PatternBase의 실제 메서드 이름인 StartPatternCycle로 수정
            _activeObjects[i].StartPatternCycle();

            var audioSource = _activeObjects[i].GetComponent<AudioSource>();

            if (audioSource == null)
            {
                audioSource = _activeObjects[i].gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
            SoundManager.Instance.BindPattern(_activeObjects[i], audioSource);
        }
    }

    private void StopCurrentPhase()
    {
        if (_activeObjects == null) return;

        for (int i = 0; i < _activeObjects.Length; i++)
        {
            if (_activeObjects[i] != null)
            {
                _activeObjects[i].StopPatternCycle();
                Destroy(_activeObjects[i].gameObject);
            }
        }
        _activeObjects = null;
    }

    public void ChangePhase()
    {
        if (_phaseIndex < _phase.Count - 1)
        {
            StopCurrentPhase();
            _curPhase = _phase[++_phaseIndex];
            StartCurrentPhase();
        }
    }

    public void TriggerCounter()
    {
        if (_activeObjects == null) return;

        foreach (PatternBase pattern in _activeObjects)
        {
            if (pattern != null)
                pattern.TriggerCounter();
        }
    }
}