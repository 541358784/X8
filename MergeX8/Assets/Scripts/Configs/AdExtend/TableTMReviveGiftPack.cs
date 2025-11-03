using System;
using System.Collections.Generic;

[System.Serializable]
public class TMReviveGiftPack : TableBase
{   
    // 编号
    public int Id { get; set; }// 礼包等级
    public int Level { get; set; }// 商品ID
    public int ShopId { get; set; }// 显示折扣百分比（1-100）; 配置100表示没有折扣
    public int Discount { get; set; }// 道具ID组
    public List<int> ItemIds { get; set; }// 道具数量组
    public List<int> ItemNums { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
