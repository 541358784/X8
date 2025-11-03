using System;
using System.Collections.Generic;

[System.Serializable]
public class MixMasterFormulaConfig : TableBase
{   
    // ID
    public int Id { get; set; }// 是否隐藏
    public bool IsHide { get; set; }// 首次制作奖励ID
    public List<int> FirstRewardId { get; set; }// 首次制作奖励数量
    public List<int> FirstRewardNum { get; set; }// 重复制作奖励ID
    public List<int> RepeatRewardId { get; set; }// 重复制作奖励数量
    public List<int> RepeatRewardNum { get; set; }// 材料ID
    public List<int> MaterialId { get; set; }// 材料数量
    public List<int> MaterialNum { get; set; }// 配方名翻译表
    public string NameKey { get; set; }// 提示翻译表
    public string TipKey { get; set; }// 图标图片
    public string Image { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
