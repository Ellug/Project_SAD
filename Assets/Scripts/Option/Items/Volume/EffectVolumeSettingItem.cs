using UnityEngine;
using UnityEngine.UI;

public class EffectVolumeSettingItem : SettingItem
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
        SettingManager.Instance.SetEffectVolume(value);
    }

    public override void Refresh()
    {
        slider.SetValueWithoutNotify(
            SettingManager.Instance.Data.effectVolume
            );
    }
}
