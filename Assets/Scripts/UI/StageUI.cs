using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [Header("Models")]
    [SerializeField] private PlayerModel _playerModel;
    [SerializeField] private BossController _bossController;

    [Header("HP UI")]
    [SerializeField] private Slider _playerHpSlider;
    [SerializeField] private Slider _bossHpSlider;

    [Header("Cooldown UI")]
    [SerializeField] private Slider _dodgeCooldownSlider;
    [SerializeField] private Slider _specialCooldownSlider;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _resultPanel;

    [Header("GameResult")]
    [SerializeField] private TMP_Text _resultText;
    [SerializeField] private Image _resultColor;
    [SerializeField] private TMP_Text _remainedBossHp;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        if (_resultPanel != null)
            _resultPanel.SetActive(false);
    }

    private void Update()
    {
        UpdatePlayerHP();
        UpdateBossHP();
        UpdateCooldowns();

        // 퍼즈, 게임 종료 임시 인풋
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            GameManager.Instance.TogglePause();
    }

    // HP Bar Update
    private void UpdatePlayerHP()
    {
        if (_playerModel == null) return;

        float ratio = _playerModel.CurHp / _playerModel.MaxHp;

        if (_playerHpSlider != null)
            _playerHpSlider.value = ratio;
    }

    private void UpdateBossHP()
    {
        if (_bossController == null) return;

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
