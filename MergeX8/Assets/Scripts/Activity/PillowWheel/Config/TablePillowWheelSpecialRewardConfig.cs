using System;
using System.Collections.Generic;

[System.Serializable]
public class PillowWheelSpecialRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 奖励ID
    public int RewardId { get; set; }// 奖励数量
    public int RewardNum { get; set; }// 收集数量
    public int Count { get; set; }// 分层组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
