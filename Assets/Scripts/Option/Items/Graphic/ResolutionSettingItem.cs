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

        int maxWidth = Screen.currentResolution.width;
        int maxHeight = Screen.currentResolution.height;

        // (width, height) 기준으로 최고 Hz만 딕셔너리로 저장
        Dictionary<(int w, int h), Resolution> filtered = new();

        foreach (var r in resolutions)
        {
            // 현재 모니터보다 큰 해상도 삭제
            if (r.width > maxWidth || r.height > maxHeight)
                continue;

            var key = (r.width, r.height);

            if (!filtered.ContainsKey(key) || r.refreshRateRatio.value > filtered[key].refreshRateRatio.value)
            {
                filtered[key] = r;
            }
        }

        List<string> options = new();

        foreach (var r in filtered.Values)
        {
            int hz = Mathf.RoundToInt((float)r.refreshRateRatio.value);
            options.Add($"{r.width} x {r.height} ({hz}Hz)");
        }

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