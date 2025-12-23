using UnityEngine;

public class UIPanelInteration : InteractionableObject
{
    [SerializeField] private GameObject _uiPanel;

    public override void OnInteract()
    {
        _interactionKey.SetActive(false);
        UIManager.Instance.OpenUI(_uiPanel);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        _uiPanel.SetActive(false);
    }  
}
