using System;
using System.Collections.Generic;

[System.Serializable]
public class SummerWatermelonProductConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 建筑ID
    public int ItemId { get; set; }// 产出内容
    public List<int> OutPut { get; set; }// 产出权重
    public List<int> Weight { get; set; }// 总权重
    public int MaxWeight { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
