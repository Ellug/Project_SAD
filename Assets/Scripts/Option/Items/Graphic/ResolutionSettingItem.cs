using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionSettingItem : SettingItem
{
    [SerializeField] private TMP_Dropdown dropdown;

    private Resolution[] resolutions;

    private void Awake()
    {
        resolutions = Screen.resolutions;

        List<string> options = new();
        foreach (var r in resolutions)
            options.Add($"{r.width} x {r.height}");

        dropdown.ClearOptions();
        dropdown.AddOptions(options);

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
        SettingManager.Instance.SetResolution(index);
    }

    public override void Refresh()
    {
        dropdown.SetValueWithoutNotify(
            SettingManager.Instance.Data.resolutionIndex
        );
    }
}