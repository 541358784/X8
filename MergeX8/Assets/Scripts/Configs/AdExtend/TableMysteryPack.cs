using System;
using System.Collections.Generic;

[System.Serializable]
public class MysteryPack : TableBase
{   
    // #
    public int Id { get; set; }// 分组
    public int Groupid { get; set; }// 名称
    public string Name { get; set; }// 描述
    public string Description { get; set; }// 购买的ID; 详见SHOP表
    public int ShopItem { get; set; }// 间隔RV次数
    public int Interval { get; set; }// 每日最大出现次数
    public int Max_times { get; set; }// 每日可购买最大次数
    public int Pay_times { get; set; }// 折扣多少(30% OFF)
    public int Discount { get; set; }// 礼包内容
    public List<int> Content { get; set; }// 道具数量
    public List<int> Count { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
