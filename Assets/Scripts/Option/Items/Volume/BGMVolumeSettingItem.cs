using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSettingItem : SettingItem
{
    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnChanged);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnChanged(float value)
    {
        SettingManager.Instance.SetBGMVolume(value);
    }

    public override void Refresh()
    {
        slider.SetValueWithoutNotify(
            SettingManager.Instance.Data.BGMVolume
            );
    }
}
