using System;
using System.Collections.Generic;

[System.Serializable]
public class BiuBiuPackageConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardCount { get; set; }// SHOPID
    public int ShopId { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
