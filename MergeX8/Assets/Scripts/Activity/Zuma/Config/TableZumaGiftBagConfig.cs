using System;
using System.Collections.Generic;

[System.Serializable]
public class ZumaGiftBagConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 商店ID
    public int ShopId { get; set; }// 奖励类型
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
