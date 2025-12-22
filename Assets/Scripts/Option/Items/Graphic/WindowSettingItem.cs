using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindowSettingItem : SettingItem
{
    [SerializeField] private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(new List<string>
        {
            "Fullscreen",
            "Windowed",
            "Borderless"
        });

        dropdown.onValueChanged.AddListener(OnChanged);
    }

    private void OnEnable()
    {
        SettingManager.Instance.OnSettingChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        SettingManager.Instance.OnSettingChanged -= Refresh;
    }

    private void OnChanged(int index)
    {
        FullScreenMode mode = index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.Windowed,
            _ => FullScreenMode.FullScreenWindow
        };

        SettingManager.Instance.SetScreenMode(mode);
    }

    public override void Refresh()
    {
        int index = SettingManager.Instance.Data.screenMode switch
        {
            FullScreenMode.ExclusiveFullScreen => 0,
            FullScreenMode.Windowed => 1,
            _ => 2
        };

        dropdown.SetValueWithoutNotify(index);
    }
}