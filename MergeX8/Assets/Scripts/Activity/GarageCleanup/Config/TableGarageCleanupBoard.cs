using System;
using System.Collections.Generic;

[System.Serializable]
public class GarageCleanupBoard : TableBase
{   
    // 
    public int Id { get; set; }// 
    public int Index { get; set; }// 
    public int ItemId { get; set; }// 阶段
    public int Level { get; set; }// 解锁奖励
    public List<int> UnlockReward { get; set; }// 解锁奖励数量
    public List<int> UnlockRewardCount { get; set; }// 鱼塘券数量
    public int Fishpondtoken { get; set; }// 等级组
    public int LevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
