using System;
using System.Collections.Generic;

[System.Serializable]
public class MasterCardList : TableBase
{   
    // ID
    public int Id { get; set; }// 7天SHOPID
    public int Buy7_shopId { get; set; }// 30天SHOPID
    public int Buy30_shopId { get; set; }// 7天初始展示价格
    public float Org7_showPrice { get; set; }// 30天初始展示价格
    public float Org30_showPrice { get; set; }// MASTERCAST 数据
    public List<int> ListData { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
