using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerModel _playerModel;

    // [Header("Boss")]
    // [SerializeField] private BossController _bossController;

    [Header("HP UI")]
    [SerializeField] private Slider _playerHpSlider;
    // [SerializeField] private Slider _bossHpSlider;

    [Header("Cooldown UI")]
    [SerializeField] private Slider _dodgeCooldownSlider;
    [SerializeField] private Slider _specialCooldownSlider;

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _resultPanel;

    private bool _isPaused = false;

    private void Start()
    {
        // 초기화 대신 최초 세팅
        UpdatePlayerHP();
        // UpdateBossHP();
        UpdateCooldowns();

        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        if (_resultPanel != null)
            _resultPanel.SetActive(false);
    }

    private void Update()
    {
        UpdatePlayerHP();
        // UpdateBossHP();
        UpdateCooldowns();

        // 퍼즈, 게임 종료 임시 인풋
        // TODO: 매니져 분리할 가능성 높아 보임
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();

        if (Keyboard.current.pKey.wasPressedThisFrame)
            ShowResult();
    }

    private void UpdatePlayerHP()
    {
        if (_playerModel == null) return;

        float ratio = _playerModel.CurHp / _playerModel.MaxHp;

        if (_playerHpSlider != null)
            _playerHpSlider.value = ratio;
    }

    // TODO : Boss HP Float으로 정리 후 연결 및 활성화
    // private void UpdateBossHP()
    // {
    //     if (_bossController == null) return;

    //     float ratio = _bossController.CurHp / _bossController.MaxHp;

    //     if (_bossHpSlider != null)
    //         _bossHpSlider.value = ratio;
    // }

    private void UpdateCooldowns()
    {
        // Dodge
        if (_dodgeCooldownSlider != null)
            _dodgeCooldownSlider.value = _playerModel.DodgeCooldownRatio;

        // Special Attack
        if (_specialCooldownSlider != null)
            _specialCooldownSlider.value = _playerModel.SpecialCooldownRatio;
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_pausePanel != null)
            _pausePanel.SetActive(_isPaused);

        Time.timeScale = _isPaused ? 0f : 1f;
    }

    // 토글로 때워도 되는데 일단 분리
    public void ResumeGame()
    {
        _isPaused = false;

        if (_pausePanel != null)
            _pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // 결과창
    public void ShowResult()
    {
        Time.timeScale = 0f;

        // TODO: 보스 남은 hp, 승리 여부 받아야함

        if (_resultPanel != null)
            _resultPanel.SetActive(true);
    }

    // 로비로 - TODO: 다른 sc와 중복된 부분 통합 고려해서 리팩토링 하면 좋음
    public void GoToLobby()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby");
    }

    // 현재 씬 재시작
    public void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TODO: 이런 것도 게임 매니져에서 중앙 관리 (캐싱할 데이터 관리 등 처리) 하면 좋아 보임
    public void GameExit() 
    {
        Application.Quit();
    }
}
