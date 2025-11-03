/************************************************
 * Config class : TreasureHuntLevelConfig
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using System;
using System.Collections.Generic;


[System.Serializable]
public class TreasureHuntLevelConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 棋盘阶段
    public int Level { get; set; }// 瓶子个数
    public int ItemCount { get; set; }// 大奖下限
    public int UnderLimit { get; set; }// 小奖
    public List<int> RandomReward { get; set; }// 数量
    public List<int> RandomRewardCount { get; set; }// 大奖
    public List<int> FinishReward { get; set; }// 数量
    public List<int> FinishRewardCount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
