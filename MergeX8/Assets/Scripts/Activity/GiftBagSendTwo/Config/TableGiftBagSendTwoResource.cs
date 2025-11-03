using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagSendTwoResource : TableBase
{   
    // #
    public int Id { get; set; }// 奖励ID
    public List<int> RewardID { get; set; }// 奖励数量; （无限体力单位为秒）
    public List<int> Amount { get; set; }// 消耗货币类型; 1 免费; 2 充值
    public int ConsumeType { get; set; }// 消耗参数; SHOP_ID
    public int ConsumeAmount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
