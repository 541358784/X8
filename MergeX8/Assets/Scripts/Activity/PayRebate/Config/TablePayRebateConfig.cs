using System;
using System.Collections.Generic;

[System.Serializable]
public class PayRebateConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 奖励物品
    public List<int> RewardID { get; set; }// 奖励物品数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
