using System;
using System.Collections.Generic;

[System.Serializable]
public class SummerWatermelonBreadMergeRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 西瓜合成链等级
    public int Level { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
