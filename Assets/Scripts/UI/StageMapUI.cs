using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapUI : MonoBehaviour
{
    [SerializeField] private GameObject _nodeDataPanel;
    [SerializeField] private TextMeshProUGUI _selectedNodeText;

    private TextMeshProUGUI[] _infoText;
    private StageNodeData _selectedStage;

    private void Start()
    {
        _infoText = _nodeDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public GameObject GetPanel()
    {
        return _nodeDataPanel;
    }

    public void SetInfoPanel(StageNodeData data)
    {
        if (_infoText != null && _infoText.Length >= 3) 
        {
            _infoText[0].text = data.StageNumber;
            _infoText[1].text = data.AreaInfo;
            _infoText[2].text = data.BossInfo;
        }
        else
        {
            Debug.LogError("지역 정보 패널이 잘못되었습니다.");
        }
    }

    public void SelectStage(StageNodeData data)
    {
        _selectedStage = data;
        _selectedNodeText.text = _selectedStage.AreaInfo;
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

        string sceneName = _selectedStage.StageNumber.Replace(" ","");

        SceneManager.LoadScene(sceneName);
    }
}
