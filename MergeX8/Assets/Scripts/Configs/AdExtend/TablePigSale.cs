using System;
using System.Collections.Generic;

[System.Serializable]
public class PigSale : TableBase
{   
    // ID
    public int Id { get; set; }// 广告组
    public int GroupId { get; set; }// 阶段（星星数）
    public int StarNum { get; set; }// 物品
    public int Item { get; set; }// 货币类型
    public int CostType { get; set; }// 原价
    public int Price { get; set; }// 每轮系数
    public List<int> Increase { get; set; }// 购买次数
    public int Times { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
