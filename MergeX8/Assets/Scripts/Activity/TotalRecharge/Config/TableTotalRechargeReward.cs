/************************************************
 * Config class : TotalRechargeReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TotalRechargeReward : TableBase
{   
    // ID
    public int Id { get; set; }// ID
    public int Group { get; set; }// 累计充值（美分）
    public int Score { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
