using System;
using System.Collections.Generic;

[System.Serializable]
public class BlindBoxGroupConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 主题
    public int ThemeId { get; set; }// 图
    public string Image { get; set; }// 翻译表
    public string NameKey { get; set; }// 组合内容
    public List<int> ItemList { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
