using System;
using System.Collections.Generic;

[System.Serializable]
public class GiftBagProgressTaskConfig : TableBase
{   
    // 编号
    public int Id { get; set; }// 收集类型(-1:戳气泡,-2:闪购,-3:获得金币)
    public int CollectType { get; set; }// 目标类型(0:收集,1:消耗,2:特殊)
    public int TargetType { get; set; }// 收集数量
    public int CollectCount { get; set; }// 奖励ID
    public List<int> RewardId { get; set; }// 奖励数量
    public List<int> RewardNum { get; set; }// 文案翻译表
    public string LabelText { get; set; }

    public override int GetID()
    {
        return Id;
    }
}
