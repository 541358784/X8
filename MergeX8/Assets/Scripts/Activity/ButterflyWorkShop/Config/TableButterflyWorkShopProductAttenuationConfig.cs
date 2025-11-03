using System;
using System.Collections.Generic;

[System.Serializable]
public class ButterflyWorkShopProductAttenuationConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 产出次数
    public int ProductCount { get; set; }// 总权重提升系数
    public float TotalWeightMulti { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
