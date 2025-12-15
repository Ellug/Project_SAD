using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutUI : MonoBehaviour
{
    [SerializeField] private Button _rifleButton;
    [SerializeField] private Button _snipeButton;
    [SerializeField] private Button _shotgunButton;
    [SerializeField] private GameObject _riflePeck;
    [SerializeField] private GameObject _snipePeck;
    [SerializeField] private GameObject _shotgunPeck;

    public event Action<Weapon> OnWeaponSelected;

    public void OnClickRifle() => Select(Weapon.Rifle);
    public void OnClickSnipe() => Select(Weapon.Snipe);
    public void OnClickShotgun() => Select(Weapon.Shotgun);

    private void Select(Weapon weapon)
    {
        SetAllInActive();

        switch (weapon)
        {
            case Weapon.Rifle:
                _riflePeck.SetActive(true);
                _rifleButton.interactable = false;
                break;
            case Weapon.Snipe:
                _snipePeck.SetActive(true);
                _snipeButton.interactable = false;
                break;
            case Weapon.Shotgun:
                _shotgunPeck.SetActive(true);
                _shotgunButton.interactable = false;
                break;
        }

        OnWeaponSelected?.Invoke(weapon);
    }

    public void SetAllInActive()
    {
        _riflePeck.SetActive(false);
        _snipePeck.SetActive(false);
        _shotgunPeck.SetActive(false);
        _rifleButton.interactable = true;
        _snipeButton.interactable = true;
        _shotgunButton.interactable = true;
    }
}
