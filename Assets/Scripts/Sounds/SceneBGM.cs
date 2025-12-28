using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private BGMEnum _bgm;

    private void Start()
    {
        if (SoundManager.Instance == null)
            return;

        SoundManager.Instance.PlayBGM(_bgm);
    }
}
