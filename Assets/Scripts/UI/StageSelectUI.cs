using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectUI : MonoBehaviour
{
    public void OnClickStageButton(string SceneName)
    {
        // TODO :
        // Maybe? 로드아웃 데이터 다루기? -> 매니져가 전담하고 씬만 변경?

        // 씬 체인지
        SceneManager.LoadScene(SceneName);
    }
}
