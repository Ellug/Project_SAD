using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutUI : MonoBehaviour
{
    [Header("WeaponButton")]
    [SerializeField] private Button _rifleButton;
    [SerializeField] private Button _snipeButton;
    [SerializeField] private Button _shotgunButton;
    [SerializeField] private GameObject _riflePeck;
    [SerializeField] private GameObject _snipePeck;
    [SerializeField] private GameObject _shotgunPeck;

    [Header("PeckNode")]
    [SerializeField] private Button Stage1Node;
    [SerializeField] private Button Stage2LeftNode;
    [SerializeField] private Button Stage2RightNode;
    [SerializeField] private Button Stage3LeftNode;
    [SerializeField] private Button Stage3RightNode;

    public event Action<WeaponBase> OnWeaponSelected;
    public event Action<StagePeck> OnStagePeckSelected;

    private Weapon _currentWeapon;

    public void OnClickRifle() => Select(Weapon.Rifle);
    public void OnClickSnipe() => Select(Weapon.Snipe);
    public void OnClickShotgun() => Select(Weapon.Shotgun);

    private void Select(Weapon weapon)
    {
        _currentWeapon = weapon;

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
        //OnWeaponSelected?.Invoke(weapon);
    }
    public void OnClickStage1() => SelectStagePeck(Stage.Stage1, Peck.None);
    public void OnClickStage2Left() => SelectStagePeck(Stage.Stage2, Peck.Left);
    public void OnClickStage2Right() => SelectStagePeck(Stage.Stage2, Peck.Right);
    public void OnClickStage3Left() => SelectStagePeck(Stage.Stage3, Peck.Left);
    public void OnClickStage3Right() => SelectStagePeck(Stage.Stage3, Peck.Right);

    private void SelectStagePeck(Stage stage, Peck peck)
    {
        //SetAllStageNodesInactive();

        if (stage == Stage.Stage1)
            Stage1Node.interactable = false;
        else if (stage == Stage.Stage2 && peck == Peck.Left)
        {
            Stage2LeftNode.interactable = false;
            Stage2RightNode.interactable = true;
        }
        else if (stage == Stage.Stage2 && peck == Peck.Right)
        {
            Stage2RightNode.interactable = false;
            Stage2LeftNode.interactable = true;
        }
        else if (stage == Stage.Stage3 && peck == Peck.Left)
        {
            Stage3LeftNode.interactable = false;
            Stage3RightNode.interactable = true;
        }
        else if (stage == Stage.Stage3 && peck == Peck.Right)
        {
            Stage3RightNode.interactable = false;
            Stage3LeftNode.interactable = true;
        }

        OnStagePeckSelected?.Invoke(new StagePeck(stage, peck));
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
