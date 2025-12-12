using UnityEngine;
using UnityEngine.UI;

public class LoadOutUI : MonoBehaviour
{
    [SerializeField] private Button _rifleButton;
    [SerializeField] private Button _snipeButton;
    [SerializeField] private Button _shotgunButton;
    [SerializeField] private GameObject _riflePuck;
    [SerializeField] private GameObject _snipePuck;
    [SerializeField] private GameObject _shotgunPuck;


    public void OnClickRifle()
    {
        SetAllInActive();
        _riflePuck.SetActive(true);
        _rifleButton.interactable = false;
    }
    public void OnClickSnipe()
    {
        SetAllInActive();
        _snipePuck.SetActive(true);
        _snipeButton.interactable = false;
    }
    public void OnClickShotgun()
    {
        SetAllInActive();
        _shotgunPuck.SetActive(true);
        _shotgunButton.interactable = false;
    }

    public void SetAllInActive()
    {
        _riflePuck.SetActive(false);
        _rifleButton.interactable = true;
        _snipePuck.SetActive(false);
        _snipeButton.interactable = true;
        _shotgunPuck.SetActive(false);
        _shotgunButton.interactable = true;
    }
}
