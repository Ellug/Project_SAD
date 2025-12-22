using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private SettingItem[] items;

    private void OnEnable()
    {
        foreach (var item in items)
            item.Refresh();
    }
}
