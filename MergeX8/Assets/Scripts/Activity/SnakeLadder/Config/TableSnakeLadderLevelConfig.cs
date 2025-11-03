using System;
using System.Collections.Generic;

[System.Serializable]
public class SnakeLadderLevelConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 需要通关次数
    public int PlayTimes { get; set; }// 格子ID列表
    public List<int> BlockList { get; set; }// 卡牌池ID
    public List<int> CardPoolId { get; set; }// 卡牌池数量
    public List<int> CardPoolNum { get; set; }// 资源名
    public string AssetName { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
