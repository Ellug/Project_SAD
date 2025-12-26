using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapUI : MonoBehaviour
{
    [SerializeField] private GameObject _nodeDataPanel;
    [SerializeField] private StageNodeData[] _stageNodeData;

    private TextMeshProUGUI[] _infoText;
    private StageNodeData _selectedStage;

    void Start()
    {
        _infoText = _nodeDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public GameObject GetPanel()
    {
        return _nodeDataPanel;
    }

    public void SetInfoPanel(StageNodeData data)
    {
        if (_infoText != null && _infoText.Length >= 2) 
        {
            _infoText[0].text = $"STAGE {data.StageNumber}";
            _infoText[1].text = data.BossInfo;
        }
        else
        {
            Debug.LogError("지역 정보 패널이 잘못되었습니다.");
        }
    }

    public void EnterStage()
    {
        if (EquipManager.Instance.Weapon == null)
        {
            Debug.Log("무기 없다 이 사람아");
            return;
        }
        if (_selectedStage == null)
        {
            Debug.Log("스테이지 선택 안했다 이 양반아");
            return;
        }

        string sceneName = "Stage"+_selectedStage.StageNumber;

        SceneManager.LoadScene(sceneName);
    }

    public void OnClickStage(int index)
    {
        _selectedStage = _stageNodeData[index];
        SetInfoPanel(_selectedStage);
    }
}
