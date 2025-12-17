using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    public void OnClickStageButton(string SceneName)
    {
        if (GameManager.Instance.Weapon == null)
        {
            Debug.Log("무기 없다 이 사람아");
            return;
        }

        SceneManager.LoadScene(SceneName);
    }
}
