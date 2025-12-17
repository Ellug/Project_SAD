using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _lobbyCanvas;

    [Header("Default UI")]
    [SerializeField] private GameObject _lobbyUI;

    private void Awake()
    {
        if (_playerController == null)
            _playerController = FindAnyObjectByType<PlayerController>();
    }

    public void OpenUI(GameObject targetUI)
    {
        _lobbyUI.SetActive(false);
        SetAllUI(false);
        targetUI.SetActive(true);

        _playerController.OpenCloseUI(true);
    }

    public void CloseUI()
    {
        GameManager gm = GameManager.Instance;
        
        if (gm.Weapon != null)
            gm.EquipPlayerWeapon();
        
        SetAllUI(false);
        _lobbyUI.SetActive(true);

        _playerController.OpenCloseUI(false);
    }

    private void SetAllUI(bool isActive)
    {
        foreach (Transform child in _lobbyCanvas.transform)
        {
            if (child.gameObject == _lobbyUI)
                continue;
            child.gameObject.SetActive(isActive);
        }
    }
}
