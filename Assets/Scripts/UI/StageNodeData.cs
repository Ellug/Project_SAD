using UnityEngine;

[CreateAssetMenu(fileName = "StageNodeData", menuName = "ScriptableObject/StageNode")]
public class StageNodeData : ScriptableObject
{
    [SerializeField] private string _stageNumber;
    [SerializeField] private Sprite _bossImage;
    [SerializeField] private string _bossInfo;

    public string StageNumber => _stageNumber;
    public Sprite BossImage => _bossImage;
    public string BossInfo => _bossInfo;
}
