using System;
using System.Collections.Generic;

[System.Serializable]
public class NewDailyPackageExtraRewardConfig : TableBase
{   
    // PACKAGEID
    public int Id { get; set; }// ITEMID
    public List<int> RewardId { get; set; }// 数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
