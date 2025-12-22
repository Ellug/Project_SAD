using UnityEngine;

[CreateAssetMenu(fileName = "StageNodeData", menuName = "ScriptableObject/StageNode")]
public class StageNodeData : ScriptableObject
{
    [SerializeField] private string _stageNumber;
    [SerializeField] private string _areaInfo;
    [SerializeField] private string _bossInfo;

    public string StageNumber => _stageNumber;
    public string AreaInfo => _areaInfo;
    public string BossInfo => _bossInfo;
}
