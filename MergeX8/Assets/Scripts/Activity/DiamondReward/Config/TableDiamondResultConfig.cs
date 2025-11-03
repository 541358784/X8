using System;
using System.Collections.Generic;

[System.Serializable]
public class DiamondResultConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 所属等级
    public int Level { get; set; }// 高付费分组; 奖励ID
    public int RewardId { get; set; }// 高付费分组; 奖励数量
    public int RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
