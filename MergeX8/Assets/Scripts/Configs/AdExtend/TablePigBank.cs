using System;
using System.Collections.Generic;

[System.Serializable]
public class PigBank : TableBase
{   
    // ID
    public int Id { get; set; }// 阶段1的值
    public int Stage_1 { get; set; }// 阶段2的值
    public int Stage_2 { get; set; }// 单次灌注数量
    public int Value { get; set; }// 商店ID
    public int ShopId { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
