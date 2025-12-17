using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    public void OnClickStageButton(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
