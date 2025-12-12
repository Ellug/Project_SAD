using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    public void OnClickStage1Button()
    {
        SceneManager.LoadScene(2);
    }
}
