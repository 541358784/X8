using System;
using System.Collections.Generic;

[System.Serializable]
public class TaskAssist : TableBase
{   
    // #
    public int Id { get; set; }// 分组
    public int RvGroup { get; set; }// 名称
    public string Name { get; set; }// 描述
    public string Description { get; set; }// 所在合成链
    public int MergeLine { get; set; }// 购买的ID; 详见SHOP表
    public int ShopItem { get; set; }// 显示索引
    public int ShopIndex { get; set; }// 持续时间(秒); -1代表无限制
    public int Duration { get; set; }// 礼包可购买次数
    public int Times { get; set; }// 折扣多少(30% OFF)
    public int Discount { get; set; }// 礼包内容
    public List<int> Content { get; set; }// 道具数量
    public List<int> Count { get; set; }// 任务目标等级
    public int TastItemLevel { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
