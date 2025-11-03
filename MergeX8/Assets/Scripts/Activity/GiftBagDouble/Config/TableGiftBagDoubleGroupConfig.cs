using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagDoubleGroupConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 分组包含礼包
    public List<int> ProductList { get; set; }// 是否一键购买
    public bool BuyAll { get; set; }// 一键购买SHOPID
    public int BuyAllShopId { get; set; }// 折扣
    public int Discount { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
