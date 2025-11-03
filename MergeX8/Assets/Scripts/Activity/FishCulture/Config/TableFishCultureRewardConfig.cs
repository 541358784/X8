using System;
using System.Collections.Generic;

[System.Serializable]
public class FishCultureRewardConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 价格
    public int Price { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 鱼编号
    public int Fish { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
