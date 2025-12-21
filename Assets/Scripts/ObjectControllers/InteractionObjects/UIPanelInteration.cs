using UnityEngine;

public class UIPanelInteration : InteractionableObject
{
    [SerializeField] private GameObject _uiPanel;

    public override void OnInteract()
    {
        _interactionKey.SetActive(false);
        _uiPanel.SetActive(true);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        _uiPanel.SetActive(false);
    }  
}
