using System;
using System.Collections.Generic;

[System.Serializable]
public class IceBreakPack : TableBase
{   
    // #
    public int Id { get; set; }// 分组
    public int RvGroup { get; set; }// 名称
    public string Name { get; set; }// 描述
    public string Description { get; set; }// 开放条件
    public int OpenLevel { get; set; }// 礼包类型
    public int Type { get; set; }// 购买的ID; 详见SHOP表
    public int ShopItem { get; set; }// 显示索引
    public int ShopIndex { get; set; }// 持续时间(秒); -1代表无限制
    public int Duration { get; set; }// 折扣多少(30% OFF)
    public int Discount { get; set; }// 礼包内容
    public List<int> Content { get; set; }// 道具数量
    public List<int> Count { get; set; }// 
    public List<int> Consume { get; set; }// 
    public string Day1 { get; set; }// 
    public string Day2 { get; set; }// 
    public string Day3 { get; set; }// 
    public string Day4 { get; set; }// 
    public string Day5 { get; set; }// 
    public string Day6 { get; set; }// 
    public string Day7 { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
