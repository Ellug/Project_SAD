using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageMapUI : MonoBehaviour
{
    [SerializeField] private GameObject _nodeDataPanel;
    [SerializeField] private StageNodeData[] _stageNodeData;
    [SerializeField] private Sprite _unknownImage;

    private TextMeshProUGUI[] _infoText;
    private Image _bossImage;
    private StageNodeData _selectedStage;

    void Start()
    {
        _infoText = _nodeDataPanel.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = _nodeDataPanel.GetComponentsInChildren<Image>();
        if (images.Length > 1)
            _bossImage = images[1];
        else
            Debug.LogError("Not Found Image Component in Stage Select UI");
    }

    private void OnDisable()
    {
        _selectedStage = null;
    }

    public void SetInfoPanel(StageNodeData data, bool isUnlock)
    {
        if (_infoText != null && _infoText.Length >= 2) 
        {
            _infoText[0].text = $"STAGE {data.StageNumber}";
            if (isUnlock)
            {
                _infoText[1].text = data.BossInfo;
                _bossImage.sprite = _selectedStage.BossImage;
            }
            else
            {
                _infoText[1].text = "잠금 상태";
                _bossImage.sprite = _unknownImage;
            }
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

        int enterStage = int.Parse(_selectedStage.StageNumber);

        if (enterStage > GameManager.Instance.UnlockStage)
        {
            Debug.Log("잠금된 스테이지다 이 필멸자야");
            return;
        }

        GameManager.Instance.EnterTheStage(enterStage);
    }

    public void OnClickStage(int index)
    {
        _selectedStage = _stageNodeData[index];
        SetInfoPanel(_selectedStage, index < GameManager.Instance.UnlockStage);
    }
}
