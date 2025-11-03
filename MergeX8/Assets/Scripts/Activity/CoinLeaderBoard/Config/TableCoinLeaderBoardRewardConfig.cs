/************************************************
 * Config class : CoinLeaderBoardRewardConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class CoinLeaderBoardRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 最小排名
    public int RankMin { get; set; }// 最大排名
    public int RankMax { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
