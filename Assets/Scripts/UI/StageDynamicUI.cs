using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StageDynamicUI : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private BossController _bossController;

    [Header("Boss UI")]
    [SerializeField] private Transform _bossHpBar;
    [SerializeField] private TextMeshProUGUI _bossHpText;
    [SerializeField] private Transform _outOfScreenBoss;

    [Header("Player UI")]
    [SerializeField] private Transform _playerIndicator;
    [SerializeField] private Transform _playerHpBar;
    [SerializeField] private Transform _dodgeCooldownBar;
    [SerializeField] private Image _specialCooldownBar;
    [SerializeField] private TextMeshProUGUI _specialCooldownText;

    [Header("Result Info")]
    [SerializeField] private TextMeshProUGUI _elapsedTime;
    [SerializeField] private TextMeshProUGUI _bossRemainHp;

    private const float OUT_OF_SCREEN_INDI_PADDING = 50f;

    private Camera _mainCam;
    private int _secondTimer;
    private Coroutine _timerCoroutine;
    private Vector3 _hpBarVector;

    void Start()
    {
        _hpBarVector = Vector3.zero;
        _mainCam = Camera.main;
        GameManager.Instance.OnGameStateChanged += GameResultProcess;
        _timerCoroutine = StartCoroutine(UpdateTimer());
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= GameResultProcess;
        StopCoroutine(_timerCoroutine);
    }

    void Update()
    {
        UpdatePlayerHP();
        UpdateBossHP();
        UpdateBossIndicator();
        UpdateCooldowns();
    }

    private void LateUpdate()
    {
        PlayerIndicatorPos();
    }

    // HP Bar Update
    private void UpdatePlayerHP()
    {
        if (_playerModel == null) return;

        _playerHpBar.transform.localPosition = HpBarCalculator(
            100f,
            _playerModel.CurHp / _playerModel.MaxHp
            );
    }

    // Boss HP Update
    private void UpdateBossHP()
    {
        if (_bossController == null) return;

        float ratio = _bossController.BossCurrentHp / _bossController.BossMaxHp;

        _bossHpBar.transform.DOLocalMove(HpBarCalculator(1000f, ratio), 0.5f).SetUpdate(true);

        _bossHpText.text = $"{Math.Ceiling(ratio * 100f)}%";
    }

    private Vector3 HpBarCalculator(float maxVal, float ratio)
    {
        _hpBarVector.x = -(maxVal - (ratio * maxVal));
        return _hpBarVector;
    }

    // Boss Indicator Update
    private void UpdateBossIndicator()
    {
        if (_bossController == null) return;

        Vector3 curPos = _mainCam.WorldToScreenPoint(_bossController.transform.position);

        // 위치가 모니터 내에 있다면
        if (curPos.x > 0 && curPos.x < Screen.width && curPos.y > 0 && curPos.y < Screen.height)
        {
            if (_outOfScreenBoss.gameObject.activeSelf)
            {
                _outOfScreenBoss.gameObject.SetActive(false);
            }
        }
        // 위치가 모니터 밖에 있다면 (화살표로 변경)
        else
        {
            _outOfScreenBoss.gameObject.SetActive(true);

            if (curPos.y <= 0)
            {
                _outOfScreenBoss.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                _outOfScreenBoss.transform.position = new Vector3(Mathf.Clamp(curPos.x, 0f, Screen.width), OUT_OF_SCREEN_INDI_PADDING, 0f);
            }
            else if (curPos.y >= Screen.height)
            {
                _outOfScreenBoss.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                _outOfScreenBoss.transform.position = new Vector3(Mathf.Clamp(curPos.x, 0f, Screen.width), Screen.height - OUT_OF_SCREEN_INDI_PADDING, 0f);
            }
            else if (curPos.x <= 0)
            {
                _outOfScreenBoss.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                _outOfScreenBoss.transform.position = new Vector3(OUT_OF_SCREEN_INDI_PADDING, Mathf.Clamp(curPos.y, 0f, Screen.height), 0f);
            }
            else if (curPos.x >= Screen.width)
            {
                _outOfScreenBoss.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                _outOfScreenBoss.transform.position = new Vector3(Screen.width - OUT_OF_SCREEN_INDI_PADDING, Mathf.Clamp(curPos.y, 0f, Screen.height), 0f);
            }
        }
    }

    // CoolTime Update
    private void UpdateCooldowns()
    {
        // Dodge
        if (_dodgeCooldownBar != null)
            _dodgeCooldownBar.transform.localPosition = HpBarCalculator(100f, _playerModel.DodgeCooldownRatio);

        float cooldownRatio = _playerModel.SpecialCooldownRatio;

        // Special Attack
        if (_specialCooldownBar != null)
            _specialCooldownBar.fillAmount = cooldownRatio;

        if (_specialCooldownText != null)
            _specialCooldownText.text = $"{(cooldownRatio * 100):F0}%";
    }

    // Player Indicator Position Update
    private void PlayerIndicatorPos()
    {
        Vector3 curPos = _mainCam.WorldToScreenPoint(_playerModel.transform.position);

        _playerIndicator.transform.position = curPos;
    }

    // Timer Update
    private IEnumerator UpdateTimer()
    {
        WaitForSeconds secondDelay = new WaitForSeconds(1f);
        _secondTimer = 0;

        yield return secondDelay;

        while (true)
        {
            _secondTimer++;
            yield return secondDelay;
        }
    }

    // 다이나믹 UI가 게임 결과에서 처리할 일.
    private void GameResultProcess(GameState state)
    {
        if (state != GameState.Result) return;

        if (_playerIndicator != null)
        {
            _playerIndicator.gameObject.SetActive(false);

            _elapsedTime.text = $"{_secondTimer / 60:D2} : {_secondTimer % 60:D2}";
            _bossRemainHp.text = $"{(_bossController.BossCurrentHp / _bossController.BossMaxHp * 100f):F2}%";
        }
    }
}
