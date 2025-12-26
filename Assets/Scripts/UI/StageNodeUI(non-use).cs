using UnityEngine;
using UnityEngine.EventSystems;

// 지도에서 노드에 커서를 올리면 툴팁을 띄우는 UI 기능
// 현재 사용되지 않음.

public class StageNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler
{
    [SerializeField] private StageNodeData _data;

    //private StageMapUI _stageMap;
    private GameObject _nodePanel;
    private Vector2 _panelPosOffset;

    private void Start()
    {
        //_stageMap = transform.parent.GetComponent<StageMapUI>();
        //_nodePanel = _stageMap.GetPanel();
        _panelPosOffset = new Vector2(40f, -40f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _nodePanel.transform.position = eventData.position + _panelPosOffset;
        //_stageMap.SetInfoPanel(_data);
        _nodePanel.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //_stageMap.SelectStage(_data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _nodePanel.SetActive(false);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if ( _nodePanel.activeSelf && _nodePanel != null ) 
        {
            _nodePanel.transform.position = eventData.position + _panelPosOffset;
        }
    }
}
