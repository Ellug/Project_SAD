using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectButtonUI : MonoBehaviour
{
    public int weaponId;
    public Button button;

    [HideInInspector] public Sprite normalSprite;
    [HideInInspector] public Sprite selectedSprite;

    private void Reset()
    {
        button = GetComponent<Button>();
    }
}
