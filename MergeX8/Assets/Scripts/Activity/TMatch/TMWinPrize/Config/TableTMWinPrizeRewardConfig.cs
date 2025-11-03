/************************************************
 * Config class : TMWinPrizeRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TMWinPrizeRewardConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 难度
    public int DifficultyMark { get; set; }// 奖励ID(TM道具ID需要乘1000)
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
