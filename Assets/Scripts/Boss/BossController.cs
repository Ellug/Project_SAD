using DamageNumbersPro;
using System;
using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("건드리지 말 것")]
    [SerializeField] private DamageNumber _dmgFont;
    [Header("보스 체력&페이즈 정의")]
    [SerializeField] private float _bossMaxHp = 100;
    [SerializeField] private float[] _changePhaseHpRate;
    private int _currentPhase;

    public float BossMaxHp { get { return _bossMaxHp; }}
    public float BossCurrentHp { get; private set; }

    // Burn
    private float _burnTickInterval = 0.5f;
    private float _burnTickAcc;
    private float _burnRemain;
    private float _burnDps;

    public event Action _phaseChange;
    public event Action _takeCounterableAttack;

    void Awake()
    {
        _currentPhase = 1;
        BossCurrentHp = _bossMaxHp;
    }
    void Update()
    {
        TickBurn(Time.deltaTime);
    }

    public void TakeDamage(float dmg, bool isCounterable)
    {
        _dmgFont.Spawn(transform.position, dmg);
        BossCurrentHp -= dmg;

        if (isCounterable)
            _takeCounterableAttack?.Invoke();

        // 페이즈 전환 조건 검사
        if (_currentPhase <= _changePhaseHpRate.Length)
        {
            if (BossCurrentHp / BossMaxHp <= _changePhaseHpRate[_currentPhase - 1])
            {
                StartCoroutine(PhaseChangeAnimation());
                _phaseChange?.Invoke();
                _currentPhase++;
            }
        }
        
        if (BossCurrentHp <= 0f)
        {
            BossCurrentHp = 0f;
            StartCoroutine(DieProcess());
        }
    }

    private IEnumerator DieProcess()
    {
        yield return null;
        Die();
    }

    private IEnumerator PhaseChangeAnimation()
    {
        GameObject armor = transform.Find($"Armor0{_currentPhase}").gameObject;
        MeshRenderer[] temp = armor.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in temp)
        {
            renderer.gameObject.AddComponent<BoxCollider>();
            Rigidbody temp2 = renderer.gameObject.AddComponent<Rigidbody>();
            temp2.mass = 3f;
            temp2.AddForce(temp2.transform.forward * 60f, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(2f);

        armor.SetActive(false);
    }

    private void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.PlayerWin();
    }

    public void ApplyBurn(float dps, float duration)
    {
        if (BossCurrentHp <= 0f) return;
        if (dps <= 0f) return;
        if (duration <= 0f) return;

        // 서로 다른 burn 수치가 들어왔을 때 높은 쪽을 선택
        _burnDps = Mathf.Max(_burnDps, dps);
        _burnRemain = Mathf.Max(_burnRemain, duration);
    }

    private void TickBurn(float dt)
    {
        if (_burnRemain <= 0f) return;
        if (BossCurrentHp <= 0f) return;

        _burnRemain -= dt;
        _burnTickAcc += dt;

        while (_burnTickAcc >= _burnTickInterval && _burnRemain > 0f)
        {
            _burnTickAcc -= _burnTickInterval;

            float tickDmg = _burnDps * _burnTickInterval;
            TakeDamage(tickDmg, isCounterable: false);
            // 여기서 애니메이션이나 렌더러 색 변경해서 시각 표현도 같이 해주면 좋을듯?
        }

        if (_burnRemain <= 0f)
        {
            _burnRemain = 0f;
            _burnDps = 0f;
            _burnTickAcc = 0f;
        }
    }
}
