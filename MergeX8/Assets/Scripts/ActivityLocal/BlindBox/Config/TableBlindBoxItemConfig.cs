using System;
using System.Collections.Generic;

[System.Serializable]
public class BlindBoxItemConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 主题
    public int ThemeId { get; set; }// 图
    public string Image { get; set; }// 灰图
    public string GrayImage { get; set; }// 是否为隐藏款
    public bool IsSpecial { get; set; }// 翻译表
    public string NameKey { get; set; }// 描述翻译表
    public string TipKey { get; set; }// 回收价值
    public int RecycleValue { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
