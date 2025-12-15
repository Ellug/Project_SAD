using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("로드할 씬 이름")][SerializeField] string SceneName;

    public void ChangeScene() 
    {
        SceneManager.LoadScene(SceneName);
    }

    public void GameExit() 
    {
        Application.Quit();
    }
}
