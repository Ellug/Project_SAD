using UnityEngine;

public class KeyGuideUI : MonoBehaviour
{
    public void OnClickExit()
    {
        UIManager.Instance.TogglePause();
    }
}
