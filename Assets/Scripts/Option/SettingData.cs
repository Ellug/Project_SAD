using UnityEngine;

[CreateAssetMenu(fileName = "Setting", menuName = "ScriptableObject/Setting")]
public class SettingData : ScriptableObject
{
    [Header("Audio")]
    public float masterVolume = 1f;
    public float effectVolume = 1f;
    public float BGMVolume = 1f;

    public float prevMasterVolume = 1f;
    public bool isMute = false;

    [Header("Graphics")]
    public int resolutionIndex;
    public FullScreenMode screenMode = FullScreenMode.FullScreenWindow;
}
