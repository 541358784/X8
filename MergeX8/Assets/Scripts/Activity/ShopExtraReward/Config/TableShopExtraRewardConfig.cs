using System;
using System.Collections.Generic;

[System.Serializable]
public class ShopExtraRewardConfig : TableBase
{   
    // SHOPID
    public int Id { get; set; }// ITEMID
    public List<int> RewardId { get; set; }// 数量
    public List<int> RewardNum { get; set; }// 购买次数
    public int BuyTimes { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
