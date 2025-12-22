using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private BossController _bossController;

    [Header("HP UI")]
    //[SerializeField] private Slider _playerHpSlider;
    [SerializeField] private Slider _bossHpSlider;
    [SerializeField] private TMP_Text _playerHpText;
    [SerializeField] private TMP_Text _bossHpText;
    [SerializeField] private Transform _bossIndicator;
    [SerializeField] private Transform _outOfScreenBoss;

    [Header("Cooldown UI")]
    [SerializeField] private Slider _dodgeCooldownSlider;
    [SerializeField] private Slider _specialCooldownSlider;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _resultPanel;

    [Header("Timer")]
    [SerializeField] private TMP_Text _timerText;

    [Header("GameResult")]
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Image _resultColor;
    [SerializeField] private TMP_Text _remainedBossHp;
    [SerializeField] private TMP_Text _elapsedTime;

    private Transform _bossPos;
    private Camera _mainCam;
    private const float OUT_OF_SCREEN_INDI_PADDING = 50f;
    private int _secondTimer;
    private Coroutine _timerCoroutine;

    void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        _timerCoroutine = StartCoroutine(UpdateTimer());
    }

    void OnDisable()
    {
        StopCoroutine(_timerCoroutine);
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void Start()
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        if (_resultPanel != null)
            _resultPanel.SetActive(false);

        _bossPos = _bossController.GetComponent<Transform>();
        _mainCam = Camera.main;
    }

    void Update()
    {
        UpdatePlayerHP();
        UpdateBossHP();
        UpdateCooldowns();

        // 퍼즈, 게임 종료 임시 인풋
        //if (Keyboard.current.escapeKey.wasPressedThisFrame)
        //    GameManager.Instance.TogglePause();
    }

    // HP Bar Update
    private void UpdatePlayerHP()
    {
        if (_playerModel == null) return;

        _playerHpText.text = $"{_playerModel.CurHp} / {_playerModel.MaxHp}";
        //float ratio = _playerModel.CurHp / _playerModel.MaxHp;

        //if (_playerHpSlider != null)
        //    _playerHpSlider.value = ratio;
    }

    private void UpdateBossHP()
    {
        if (_bossController == null) return;

        Vector3 curPos = _mainCam.WorldToScreenPoint(_bossController.transform.position);

        // 위치가 모니터 내에 있다면
        if (curPos.x > 0 && curPos.x < Screen.width && curPos.y > 0 && curPos.y < Screen.height)
        {
            if(_outOfScreenBoss.gameObject.activeSelf)
            {
                _bossIndicator.gameObject.SetActive(true);
                _outOfScreenBoss.gameObject.SetActive(false);
            }
            _bossIndicator.transform.position = curPos;
            _bossHpText.text = $"BOSS -- {_bossController.BossCurrentHp / _bossController.BossMaxHp * 100f}%";
        }
        // 위치가 모니터 밖에 있다면 (화살표로 변경)
        else
        {
            if(_bossIndicator.gameObject.activeSelf)
            {
                _bossIndicator.gameObject.SetActive(false);
                _outOfScreenBoss.gameObject.SetActive(true);
            }
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
        

        float ratio = _bossController.BossCurrentHp / _bossController.BossMaxHp;

        if (_bossHpSlider != null)
            _bossHpSlider.value = ratio;
    }

    // CoolTime Update
    private void UpdateCooldowns()
    {
        // Dodge
        if (_dodgeCooldownSlider != null)
            _dodgeCooldownSlider.value = _playerModel.DodgeCooldownRatio;

        // Special Attack
        if (_specialCooldownSlider != null)
            _specialCooldownSlider.value = _playerModel.SpecialCooldownRatio;
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
            _timerText.text = $"{_secondTimer / 60:D2} : {_secondTimer % 60:D2}";
            yield return secondDelay;
        }
    }


    // 게임 상태 변경 -> 이벤트 등록
    private void HandleGameStateChanged(GameState state)
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(state == GameState.Paused);

        if (_resultPanel != null)
        {
            bool isResult = state == GameState.Result;
            _resultPanel.SetActive(isResult);

            if (isResult)
                UpdateResultUI();
        }
    }

    // 결과 UI 업데이트
    private void UpdateResultUI()
    {
        if (_resultText == null || _resultColor == null || _bossController == null)
            return;

        bool isWin = GameManager.Instance.IsPlayerWin;

        _resultText.text = isWin ? "성공" : "실패";
        _resultText.color = isWin ? Color.black : Color.white;

        _resultColor.color = isWin
            ? new Color(0.4f, 0.8352942f, 0.4627451f)
            : new Color(0.9450981f, 0.282353f, 0.1294118f);

        // 보스 남은 체력
        if (_remainedBossHp != null)
        {
            float ratio = _bossController.BossCurrentHp / _bossController.BossMaxHp * 100f;
            _remainedBossHp.text = $"남은 보스의 체력: {ratio:F1}%";
        }
        // 소요 시간 표시
        if (_elapsedTime != null)
        {
            _elapsedTime.text = $"소요 시간: {_secondTimer / 60:D2}분 {_secondTimer % 60:D2}초";
        }
    }

    // UI Btn OnClick Events
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void GoToLobby()
    {
        GameManager.Instance.GoToLobby();
    }

    public void Retry()
    {
        GameManager.Instance.ReloadCurrentScene();
    }

    public void GameExit()
    {
        GameManager.Instance.GameExit();
    }
}
