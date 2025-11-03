using System;
using System.Collections.Generic;

[System.Serializable]
public class StageReward : TableBase
{   
    // ID
    public int Id { get; set; }// 兑换次数
    public int ExchangeTimes { get; set; }// 奖励ID
    public int RewardId { get; set; }// 奖励数量
    public int RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
