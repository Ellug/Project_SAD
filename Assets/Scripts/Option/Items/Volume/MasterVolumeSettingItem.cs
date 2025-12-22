using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeSettingItem : SettingItem
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnChanged);
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

    private void OnChanged(float value)
    {
        SettingManager.Instance.SetMasterVolume(value);

        if (SettingManager.Instance.Data.isMute && value > 0f)
            SettingManager.Instance.SetMute(false);
    }

    public override void Refresh()
    {
        slider.SetValueWithoutNotify(
            SettingManager.Instance.Data.masterVolume
        );
    }
}
