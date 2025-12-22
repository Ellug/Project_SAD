using UnityEngine;
using UnityEngine.UI;

public class MuteSettingItem : SettingItem
{
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnChanged);
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

    private void OnChanged(bool isOn)
    {
        SettingManager.Instance.SetMute(isOn);
    }

    public override void Refresh()
    {
        toggle.SetIsOnWithoutNotify(
            SettingManager.Instance.Data.isMute
        );
    }
}
