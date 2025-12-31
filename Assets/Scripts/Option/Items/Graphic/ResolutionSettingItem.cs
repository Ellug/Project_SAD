using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionSettingItem : SettingItem
{
    [SerializeField] private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown.onValueChanged.AddListener(OnChanged);
    }

    private void OnEnable()
    {
        SettingManager.Instance.OnSettingChanged += Refresh;

        RebuildOptionsFromManager();
        Refresh();
    }

    private void OnDisable()
    {
        SettingManager.Instance.OnSettingChanged -= Refresh;
    }

    private void RebuildOptionsFromManager()
    {
        var list = SettingManager.Instance.AvailableResolutions;

        List<string> options = new(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            var r = list[i];
            int hz = Mathf.RoundToInt((float)r.refreshRateRatio.value);
            options.Add($"{r.width} x {r.height} ({hz}Hz)");
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    private void OnChanged(int index)
    {
        SettingManager.Instance.SetResolution(index);
    }

    public override void Refresh()
    {
        int idx = SettingManager.Instance.Data.resolutionIndex;
        dropdown.SetValueWithoutNotify(idx);
    }
}
