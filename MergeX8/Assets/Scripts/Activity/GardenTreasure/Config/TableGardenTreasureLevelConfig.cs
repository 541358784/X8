using System;
using System.Collections.Generic;

[System.Serializable]
public class GardenTreasureLevelConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 是否是随机关卡
    public bool IsRandom { get; set; }// 棋盘配置
    public List<int> BoardConfigIds { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 分层
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
