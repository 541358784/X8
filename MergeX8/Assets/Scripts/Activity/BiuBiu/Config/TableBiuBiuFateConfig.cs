using System;
using System.Collections.Generic;

[System.Serializable]
public class BiuBiuFateConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 序列
    public List<int> Fate { get; set; }// 轮次
    public int Round { get; set; }// 权重
    public int Weight { get; set; }// 分组
    public int PayLevelGroup { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
