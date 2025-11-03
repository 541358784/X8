using System;
using System.Collections.Generic;

[System.Serializable]
public class BlindBoxBoxConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 主题
    public int ThemeId { get; set; }// 权重列表
    public List<int> WeightList { get; set; }// 奖励列表
    public List<int> ItemList { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
