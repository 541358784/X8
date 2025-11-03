using System;
using System.Collections.Generic;

[System.Serializable]
public class MixMasterMixTaskConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 收集数量
    public int CollectCount { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 解锁隐藏配方ID
    public int FormulaId { get; set; }// 文案翻译表
    public string LabelText { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
