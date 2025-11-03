using System;
using System.Collections.Generic;

[System.Serializable]
public class KeepPetThreeOneStoreConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardCount { get; set; }// SHOPID
    public int ShopId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
