using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : SingletonePattern<UIManager>
{
    private PlayerInput _inputSystem;
    private Stack<GameObject> _uiStack;

    public event Action PauseUItrigger;
    public event Action AllUIClosed;

    protected override void Awake()
    {
        base.Awake();
        _uiStack = new Stack<GameObject>();
    }

    private void Start()
    {
        SearchPlayer();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드 될 때마다 플레이어를 찾는다.
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SearchPlayer();
        Init();
    }

    // 플레이어가 esc 키를 눌렀을 때 실행되는 메서드.
    // UI가 떠 있다면 최상단 UI를 닫습니다.
    // UI가 떠있지 않다면 일시정지 패널을 띄웁니다.
    public void TogglePause()
    {
        if (_uiStack.Count == 0)
            PauseUItrigger?.Invoke();
        else
            CloseTopUI();
    }

    // UI를 활성화하고 스택에 담는다.
    // 첫 UI가 열리면 액션맵도 변경.
    public void OpenUI(GameObject panel)
    {
        if (panel != null) 
        {
            if (_inputSystem != null && _uiStack.Count == 0)
            {
                GameManager.Instance.TogglePause();
                _inputSystem.SwitchCurrentActionMap("UI");
            }
            if (_uiStack.TryPeek(out GameObject prev))
                prev.SetActive(false);
            _uiStack.Push(panel);
            panel.SetActive(true);
        }
    }

    // 최상단 UI를 끈다.
    public void CloseTopUI()
    {
        if (_uiStack.Count > 0)
            _uiStack.Pop().SetActive(false);
        if (_uiStack.TryPeek(out GameObject prev))
            prev.SetActive(true);
        else
        {
            if (_inputSystem != null)
            {
                GameManager.Instance.TogglePause();
                AllUIClosed?.Invoke();
                _inputSystem.SwitchCurrentActionMap("Player");
            }
        }
    }

    public bool IsUIPopUp()
    {
        if (_uiStack.Count > 0)
            return true;
        return false;
    }

    private void Init()
    {
        if (_uiStack.Count > 0)
        {
            _uiStack.Clear();
            GameManager.Instance.ResumeGame();
        }
    }

    private void SearchPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        // 플레이어가 있다면 플레이어 인풋을 가져오고 없다면 null 할당
        if (player != null)
            _inputSystem = player.GetComponent<PlayerInput>();
        else
            _inputSystem = null;

        if (_inputSystem != null)
            _inputSystem.SwitchCurrentActionMap("Player");
    }
}
