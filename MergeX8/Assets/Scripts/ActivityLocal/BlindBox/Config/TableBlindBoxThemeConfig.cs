using System;
using System.Collections.Generic;

[System.Serializable]
public class BlindBoxThemeConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 盲盒ID
    public int BoxId { get; set; }// 棋子列表
    public List<int> ItemList { get; set; }// 奖励组合列表
    public List<int> GroupList { get; set; }// 图集名
    public string AtlasName { get; set; }// 翻译表
    public string NameKey { get; set; }// 隐藏款保底次数
    public List<int> SpecialTimes { get; set; }// 反向保底
    public List<int> RuleMinTimes { get; set; }// 正向保底
    public List<int> RuleMaxTimes { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
