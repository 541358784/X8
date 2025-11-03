using System;
using System.Collections.Generic;

[System.Serializable]
public class GarageCleanupConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 棋盘阶段
    public int Level { get; set; }// 行奖励
    public List<int> RowReward { get; set; }// 行奖励数量
    public List<int> RowRewardCount { get; set; }// 斜线奖励
    public List<int> DiagonalsReward { get; set; }// 斜线奖励数量
    public List<int> DiagonalsRewardCount { get; set; }// 全屏奖励
    public List<int> FullReward { get; set; }// 全屏奖励数量
    public List<int> FullRewardCount { get; set; }// 解锁消耗
    public List<int> UnlockConsume { get; set; }// 等级组
    public int LevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
