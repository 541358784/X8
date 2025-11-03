using System;
using System.Collections.Generic;

[System.Serializable]
public class SummerWatermelonPackageConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 计费点ID
    public int ShopId { get; set; }// 活动开启多少小时后开放
    public float StartHourCount { get; set; }// 活动开启多少小时后消失
    public float EndHourCount { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 权益系数(仅显示)
    public float LabelNum { get; set; }// 限购次数
    public int BuyLimit { get; set; }// 最大钻石
    public int MaxDiamond { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
