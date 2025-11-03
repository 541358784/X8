using System;
using System.Collections.Generic;

[System.Serializable]
public class ExchangeReward : TableBase
{   
    // ID
    public int Id { get; set; }// 兑换分数
    public int ExchangeScore { get; set; }// 奖励ID
    public int RewardId { get; set; }// 
    public string Image { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
