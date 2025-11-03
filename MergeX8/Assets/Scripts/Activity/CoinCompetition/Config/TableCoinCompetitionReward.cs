/************************************************
 * Config class : CoinCompetitionReward
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CoinCompetitionReward : TableBase
{   
    // ID
    public int Id { get; set; }// 阶段分数 总分
    public int Score { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 收集时显示的奖励
    public int RewardShowIndex { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
